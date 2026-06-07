namespace MiniZinc.TestSpec.Yaml;

using System.Text;

/// <summary>
/// Hand-rolled YAML scanner for the subset of YAML used by libminizinc
/// test files. Produces a <see cref="YamlNode"/> tree per document.
///
/// Supports:
///  - block-style mappings and sequences with significant indentation
///  - flow-style sequences <c>[a, b]</c> and mappings <c>{a, b}</c> (single line)
///  - tagged scalars / mappings / sequences (e.g. <c>!Result</c>, <c>!!set</c>)
///  - single- and double-quoted scalars; block-literal <c>|</c> and folded <c>&gt;</c>
///  - <c>---</c> document separators (optionally followed by a tag)
///  - <c>#</c> comments
///
/// Deliberately does NOT support: anchors/aliases, document end <c>...</c>,
/// directives, complex mapping keys, block-scalar chomping modifiers.
/// </summary>
public sealed class YamlScanner
{
    private readonly string _text;
    private readonly string? _path;
    private int _pos;
    private int _line = 1;
    private int _col = 1;

    public YamlScanner(string text, string? path = null)
    {
        _text = text;
        _path = path;
    }

    public IReadOnlyList<YamlNode> ParseStream()
    {
        var docs = new List<YamlNode>();

        while (true)
        {
            if (!SkipBlankAndCommentLines())
                break;

            // Optional document separator (column 0)
            string? docTag = null;
            int docLine = _line;
            int docCol = _col;
            if (AtDocSeparator())
            {
                // Consume "---"
                Advance(3);
                SkipSpacesOnCurrentLine();
                // After "---" there may be a tag on the same line
                if (_pos < _text.Length && _text[_pos] == '!')
                {
                    docTag = ReadTag();
                    SkipSpacesOnCurrentLine();
                }
                ConsumeNewline();
                if (!SkipBlankAndCommentLines())
                    break;
            }

            // Parse the document value at "outer" indent of -1
            // so anything at column >= 0 is a child.
            int firstIndent = _col - 1;
            var node = ParseValue(-1, docTag, docLine, docCol);
            docs.Add(node);
        }

        return docs;
    }

    // -------- Indent / line plumbing --------

    private void Advance(int n = 1)
    {
        for (int i = 0; i < n && _pos < _text.Length; i++)
        {
            char c = _text[_pos];
            if (c == '\n')
            {
                _line++;
                _col = 1;
            }
            else if (c == '\r')
            {
                // Don't bump line yet — if \r\n, the \n will bump.
                _col = 1;
            }
            else
            {
                _col++;
            }
            _pos++;
        }
    }

    private void ConsumeNewline()
    {
        if (_pos < _text.Length && _text[_pos] == '\r')
            Advance();
        if (_pos < _text.Length && _text[_pos] == '\n')
            Advance();
    }

    private void SkipSpacesOnCurrentLine()
    {
        while (_pos < _text.Length && _text[_pos] == ' ')
            Advance();
    }

    /// <summary>Skip blank lines and comment-only lines. Returns false at EOF.</summary>
    private bool SkipBlankAndCommentLines()
    {
        while (_pos < _text.Length)
        {
            int savedPos = _pos;
            int savedLine = _line;
            int savedCol = _col;
            SkipSpacesOnCurrentLine();
            if (_pos >= _text.Length)
                return false;

            char c = _text[_pos];
            if (c == '\n' || c == '\r')
            {
                ConsumeNewline();
                continue;
            }
            if (c == '#')
            {
                // Skip rest of line
                while (_pos < _text.Length && _text[_pos] != '\n' && _text[_pos] != '\r')
                    Advance();
                ConsumeNewline();
                continue;
            }
            // Non-blank, non-comment content; rewind to start of line
            _pos = savedPos;
            _line = savedLine;
            _col = savedCol;
            SkipSpacesOnCurrentLine();
            return true;
        }
        return false;
    }

    /// <summary>Peek the indent (column - 1) and starting line of the next
    /// significant line, without consuming. Returns false at EOF.</summary>
    private bool TryPeekNextLine(out int indent, out int peekLine)
    {
        int savedPos = _pos;
        int savedLine = _line;
        int savedCol = _col;
        bool ok = SkipBlankAndCommentLines();
        if (!ok)
        {
            indent = -1;
            peekLine = -1;
            _pos = savedPos;
            _line = savedLine;
            _col = savedCol;
            return false;
        }
        indent = _col - 1;
        peekLine = _line;
        _pos = savedPos;
        _line = savedLine;
        _col = savedCol;
        return true;
    }

    private bool AtDocSeparator()
    {
        if (_col != 1)
            return false;
        if (_pos + 2 >= _text.Length)
            return false;
        if (_text[_pos] != '-' || _text[_pos + 1] != '-' || _text[_pos + 2] != '-')
            return false;
        // Must be followed by space, EOL, or tag
        if (_pos + 3 >= _text.Length)
            return true;
        char after = _text[_pos + 3];
        return after == ' ' || after == '\n' || after == '\r' || after == '\t';
    }

    private bool LineHasMappingColon()
    {
        // Scan from _pos to end of line: look for ": " or ":" at end-of-line,
        // outside quotes and flow constructs.
        int p = _pos;
        int depth = 0;
        char? quote = null;
        while (p < _text.Length)
        {
            char c = _text[p];
            if (c == '\n' || c == '\r')
                return false;
            if (quote is char qc)
            {
                if (c == qc)
                {
                    // double-quoted handles escape; single-quoted doesn't
                    if (qc == '"' && p > _pos && _text[p - 1] == '\\')
                    {
                        p++;
                        continue;
                    }
                    quote = null;
                }
                p++;
                continue;
            }
            if (c == '\'' || c == '"')
            {
                quote = c;
                p++;
                continue;
            }
            if (c == '[' || c == '{')
                depth++;
            else if (c == ']' || c == '}')
                depth--;
            else if (depth == 0 && c == ':')
            {
                if (p + 1 >= _text.Length)
                    return true;
                char next = _text[p + 1];
                if (next == ' ' || next == '\n' || next == '\r' || next == '\t')
                    return true;
            }
            else if (depth == 0 && c == '#' && p > _pos && _text[p - 1] == ' ')
            {
                return false;
            }
            p++;
        }
        return false;
    }

    // -------- Value dispatch --------

    private YamlNode ParseValue(int parentIndent, string? carryTag, int tagLine, int tagCol)
    {
        // Caller has either positioned at start-of-line at indent > parentIndent,
        // or has a carryTag from inline (e.g. "- !Tag\n   ...").
        // Read any tag at the current position first (highest-priority).
        SkipSpacesOnCurrentLine();
        string? tag = carryTag;
        int line = _line;
        int col = _col;

        if (_pos < _text.Length && _text[_pos] == '!')
        {
            if (tag != null)
                throw Err("multiple tags on the same value");
            tag = ReadTag();
            SkipSpacesOnCurrentLine();
            line = tagLine == 0 ? _line : tagLine;
            col = tagCol == 0 ? _col : tagCol;
        }

        if (_pos >= _text.Length)
            return new YamlScalar(line, col, tag, "", ScalarStyle.Plain);

        char c = _text[_pos];

        // End-of-line after tag: the value starts on the next line.
        if (c == '\n' || c == '\r' || c == '#')
        {
            ConsumeNewline();
            if (c == '#')
            {
                // We hit a comment; consume rest of line
                while (_pos < _text.Length && _text[_pos] != '\n' && _text[_pos] != '\r')
                    Advance();
                ConsumeNewline();
            }
            return ParseBlockValue(parentIndent, tag, line, col);
        }

        // Flow constructs
        if (c == '[')
            return ParseFlowSequence(tag, line, col);
        if (c == '{')
            return ParseFlowMapping(tag, line, col);

        // Block scalar markers
        if (c == '|' || c == '>')
        {
            char marker = c;
            Advance();
            // Skip rest of marker line (no chomping indicators supported, but tolerate trailing chars)
            while (_pos < _text.Length && _text[_pos] != '\n' && _text[_pos] != '\r')
                Advance();
            ConsumeNewline();
            return ParseBlockScalar(parentIndent, marker, tag, line, col);
        }

        // Otherwise: either inline scalar (when no further content) or block container.
        // Check if this line is a "key: ..." form → block mapping starting on this line.
        if (LineHasMappingColon())
        {
            // Block mapping starts here (col is the indent of the key)
            return ParseBlockMapping(col - 1, tag, line, col);
        }

        // Inline scalar — read until EOL.
        return ParseInlineScalar(tag, line, col);
    }

    /// Called when a tag/dash was on its own line and the value is expected to
    /// start on the next line (block container or block scalar).
    /// YAML allows block sequences to be at the same indent as their parent
    /// mapping key (compact notation); other constructs must be strictly deeper.
    private YamlNode ParseBlockValue(int parentIndent, string? tag, int line, int col)
    {
        if (!TryPeekNextLine(out int childIndent, out _))
            return new YamlScalar(line, col, tag, "", ScalarStyle.Plain);

        if (childIndent < parentIndent)
            return new YamlScalar(line, col, tag, "", ScalarStyle.Plain);

        if (!SkipBlankAndCommentLines())
            return new YamlScalar(line, col, tag, "", ScalarStyle.Plain);

        char c = _text[_pos];

        if (childIndent == parentIndent)
        {
            // Compact block sequence is the only construct allowed at same indent.
            if (c == '-' && IsBlockSequenceDash())
                return ParseBlockSequence(childIndent, tag, line, col);
            // Otherwise this is a sibling, not a child — return empty.
            return new YamlScalar(line, col, tag, "", ScalarStyle.Plain);
        }

        // childIndent > parentIndent
        if (c == '-' && IsBlockSequenceDash())
            return ParseBlockSequence(childIndent, tag, line, col);

        if (LineHasMappingColon())
            return ParseBlockMapping(childIndent, tag, line, col);

        if (c == '|' || c == '>')
        {
            char marker = c;
            int sLine = _line;
            int sCol = _col;
            Advance();
            while (_pos < _text.Length && _text[_pos] != '\n' && _text[_pos] != '\r')
                Advance();
            ConsumeNewline();
            return ParseBlockScalar(parentIndent, marker, tag, sLine, sCol);
        }

        return ParseInlineScalar(tag, _line, _col);
    }

    private bool IsBlockSequenceDash()
    {
        // _pos at '-'. Confirm next char is space or EOL → block sequence indicator.
        if (_pos + 1 >= _text.Length)
            return true;
        char next = _text[_pos + 1];
        return next == ' ' || next == '\n' || next == '\r' || next == '\t';
    }

    // -------- Block sequence --------

    private YamlSequence ParseBlockSequence(int indent, string? tag, int line, int col)
    {
        var items = new List<YamlNode>();
        while (true)
        {
            if (!SkipBlankAndCommentLines())
                break;
            if (_col - 1 != indent)
                break;
            if (_pos >= _text.Length)
                break;
            if (_text[_pos] != '-' || !IsBlockSequenceDash())
                break;
            items.Add(ParseBlockSequenceItem(indent));
        }
        return new YamlSequence(line, col, tag, items);
    }

    /// <summary>
    /// Reads the value of a single sequence item. <c>_pos</c> must be at the
    /// dash. Handles inline content, including nested compact sequences
    /// (<c>- - - - 1</c>), block sub-containers on subsequent lines, etc.
    /// </summary>
    private YamlNode ParseBlockSequenceItem(int dashIndent)
    {
        int itemLine = _line;
        int itemCol = _col;
        // Consume '-' and following whitespace
        Advance();
        if (_pos < _text.Length && (_text[_pos] == ' ' || _text[_pos] == '\t'))
            Advance();
        SkipSpacesOnCurrentLine();

        // Empty item value (rest of line blank or comment-only): value is on next line
        if (_pos >= _text.Length || _text[_pos] == '\n' || _text[_pos] == '\r' || _text[_pos] == '#')
        {
            if (_pos < _text.Length && _text[_pos] == '#')
                while (_pos < _text.Length && _text[_pos] != '\n' && _text[_pos] != '\r')
                    Advance();
            ConsumeNewline();
            return ParseBlockValue(dashIndent, null, itemLine, itemCol);
        }

        // Optional tag inline after "- "
        string? innerTag = null;
        int innerLine = _line;
        int innerCol = _col;
        if (_text[_pos] == '!')
        {
            innerTag = ReadTag();
            SkipSpacesOnCurrentLine();
            if (_pos >= _text.Length || _text[_pos] == '\n' || _text[_pos] == '\r' || _text[_pos] == '#')
            {
                if (_pos < _text.Length && _text[_pos] == '#')
                    while (_pos < _text.Length && _text[_pos] != '\n' && _text[_pos] != '\r')
                        Advance();
                ConsumeNewline();
                return ParseBlockValue(dashIndent, innerTag, innerLine, innerCol);
            }
        }

        // Nested compact block sequence: another dash on the same line.
        if (_text[_pos] == '-' && IsBlockSequenceDash())
        {
            int nestedIndent = _col - 1;
            int nLine = _line;
            int nCol = _col;
            var nestedItems = new List<YamlNode>();
            nestedItems.Add(ParseBlockSequenceItem(nestedIndent));
            // Continue with sibling items at the same indent on subsequent lines.
            while (true)
            {
                if (!SkipBlankAndCommentLines())
                    break;
                if (_col - 1 != nestedIndent)
                    break;
                if (_pos >= _text.Length || _text[_pos] != '-' || !IsBlockSequenceDash())
                    break;
                nestedItems.Add(ParseBlockSequenceItem(nestedIndent));
            }
            return new YamlSequence(nLine, nCol, innerTag, nestedItems);
        }

        if (_text[_pos] == '[')
            return ParseFlowSequence(innerTag, _line, _col);
        if (_text[_pos] == '{')
            return ParseFlowMapping(innerTag, _line, _col);
        if (LineHasMappingColon())
            return ParseBlockMapping(_col - 1, innerTag, _line, _col);
        if (_text[_pos] == '|' || _text[_pos] == '>')
        {
            char m = _text[_pos];
            int sL = _line;
            int sC = _col;
            Advance();
            while (_pos < _text.Length && _text[_pos] != '\n' && _text[_pos] != '\r')
                Advance();
            ConsumeNewline();
            return ParseBlockScalar(dashIndent, m, innerTag, sL, sC);
        }
        return ParseInlineScalar(innerTag, _line, _col);
    }

    // -------- Block mapping --------

    private YamlMapping ParseBlockMapping(int indent, string? tag, int line, int col)
    {
        var entries = new List<YamlMappingEntry>();
        // PyYAML (used by the upstream libminizinc harness) tolerates duplicate
        // mapping keys with last-wins semantics rather than erroring, so we mirror
        // that: a repeated key overwrites the earlier entry in place.
        var keyIndex = new Dictionary<string, int>(StringComparer.Ordinal);
        while (true)
        {
            if (!SkipBlankAndCommentLines())
                break;
            int curIndent = _col - 1;
            if (curIndent != indent)
                break;
            if (_pos >= _text.Length)
                break;
            // Stop at document separator
            if (AtDocSeparator())
                break;
            // If this line is a sequence dash, it's not a mapping entry — stop.
            if (_text[_pos] == '-' && IsBlockSequenceDash())
                break;

            int keyLine = _line;
            int keyCol = _col;
            string keyText = ReadKeyText();
            // Expect ':'
            if (_pos >= _text.Length || _text[_pos] != ':')
                throw Err($"expected ':' after key '{keyText}'");
            Advance();
            // ':' may be followed by space/EOL
            if (_pos < _text.Length && (_text[_pos] == ' ' || _text[_pos] == '\t'))
                SkipSpacesOnCurrentLine();

            var keyScalar = new YamlScalar(keyLine, keyCol, null, keyText, ScalarStyle.Plain);

            // Value: inline (rest of line) or on a deeper-indented next line.
            YamlNode value;
            if (_pos >= _text.Length || _text[_pos] == '\n' || _text[_pos] == '\r' || _text[_pos] == '#')
            {
                if (_pos < _text.Length && _text[_pos] == '#')
                    while (_pos < _text.Length && _text[_pos] != '\n' && _text[_pos] != '\r')
                        Advance();
                ConsumeNewline();
                value = ParseBlockValue(indent, null, _line, _col);
            }
            else
            {
                value = ParseValue(indent, null, _line, _col);
            }

            var entry = new YamlMappingEntry(keyScalar, value);
            if (keyIndex.TryGetValue(keyText, out int existing))
                entries[existing] = entry;
            else
            {
                keyIndex[keyText] = entries.Count;
                entries.Add(entry);
            }
        }

        return new YamlMapping(line, col, tag, entries);
    }

    private string ReadKeyText()
    {
        if (_pos < _text.Length && (_text[_pos] == '\'' || _text[_pos] == '"'))
        {
            char q = _text[_pos];
            return ReadQuotedScalar(q);
        }
        // Plain key: read until ':' (followed by space/EOL) or EOL.
        var sb = new StringBuilder();
        int depth = 0;
        while (_pos < _text.Length)
        {
            char c = _text[_pos];
            if (c == '\n' || c == '\r')
                break;
            if (depth == 0 && c == ':')
            {
                if (_pos + 1 >= _text.Length)
                    break;
                char next = _text[_pos + 1];
                if (next == ' ' || next == '\t' || next == '\n' || next == '\r')
                    break;
            }
            if (c == '[' || c == '{')
                depth++;
            else if (c == ']' || c == '}')
                depth--;
            sb.Append(c);
            Advance();
        }
        return sb.ToString().TrimEnd();
    }

    // -------- Scalars --------

    private YamlScalar ParseInlineScalar(string? tag, int line, int col)
    {
        if (_pos < _text.Length && _text[_pos] == '\'')
        {
            string raw = ReadQuotedScalar('\'');
            return new YamlScalar(line, col, tag, raw, ScalarStyle.SingleQuoted);
        }
        if (_pos < _text.Length && _text[_pos] == '"')
        {
            string raw = ReadQuotedScalar('"');
            return new YamlScalar(line, col, tag, raw, ScalarStyle.DoubleQuoted);
        }
        // Plain scalar: until end-of-line (trim trailing spaces), excluding " # " comment.
        var sb = new StringBuilder();
        while (_pos < _text.Length)
        {
            char c = _text[_pos];
            if (c == '\n' || c == '\r')
                break;
            if (c == '#' && sb.Length > 0 && sb[sb.Length - 1] == ' ')
            {
                // Strip trailing space and stop
                while (sb.Length > 0 && sb[sb.Length - 1] == ' ')
                    sb.Length--;
                break;
            }
            sb.Append(c);
            Advance();
        }
        return new YamlScalar(line, col, tag, sb.ToString().TrimEnd(), ScalarStyle.Plain);
    }

    private string ReadQuotedScalar(char quote)
    {
        // Advance past opening quote
        Advance();
        var sb = new StringBuilder();
        while (_pos < _text.Length)
        {
            char c = _text[_pos];
            if (c == quote)
            {
                // Single-quote: '' is an escaped single quote
                if (quote == '\'' && _pos + 1 < _text.Length && _text[_pos + 1] == '\'')
                {
                    sb.Append('\'');
                    Advance();
                    Advance();
                    continue;
                }
                Advance();
                return sb.ToString();
            }
            if (quote == '"' && c == '\\')
            {
                // Escape sequences
                Advance();
                if (_pos >= _text.Length)
                    throw Err("unterminated escape in double-quoted string");
                char esc = _text[_pos];
                Advance();
                switch (esc)
                {
                    case 'n': sb.Append('\n'); break;
                    case 't': sb.Append('\t'); break;
                    case 'r': sb.Append('\r'); break;
                    case '0': sb.Append('\0'); break;
                    case 'a': sb.Append('\a'); break;
                    case 'b': sb.Append('\b'); break;
                    case 'f': sb.Append('\f'); break;
                    case 'v': sb.Append('\v'); break;
                    case '\\': sb.Append('\\'); break;
                    case '/': sb.Append('/'); break;
                    case '"': sb.Append('"'); break;
                    case '\'': sb.Append('\''); break;
                    default: sb.Append(esc); break;
                }
                continue;
            }
            sb.Append(c);
            Advance();
        }
        throw Err($"unterminated {(quote == '\'' ? "single" : "double")}-quoted string");
    }

    private YamlScalar ParseBlockScalar(int parentIndent, char marker, string? tag, int line, int col)
    {
        // Read subsequent lines whose indent > parentIndent.
        // The first non-blank such line establishes the block's indent.
        var sb = new StringBuilder();
        int blockIndent = -1;
        bool firstLine = true;
        bool prevWasBlank = false;

        while (_pos < _text.Length)
        {
            int lineStart = _pos;
            int lineStartLine = _line;
            int lineStartCol = _col;
            int leadingSpaces = 0;
            while (_pos < _text.Length && _text[_pos] == ' ')
            {
                Advance();
                leadingSpaces++;
            }
            if (_pos >= _text.Length || _text[_pos] == '\n' || _text[_pos] == '\r')
            {
                // Blank line — record and continue
                if (!firstLine)
                {
                    sb.Append('\n');
                    prevWasBlank = true;
                }
                ConsumeNewline();
                continue;
            }

            if (blockIndent == -1)
            {
                // Establishing line
                blockIndent = leadingSpaces;
                if (blockIndent <= parentIndent)
                {
                    // Block was empty — rewind and stop.
                    _pos = lineStart;
                    _line = lineStartLine;
                    _col = lineStartCol;
                    break;
                }
            }
            else if (leadingSpaces < blockIndent)
            {
                // Dedent — block ends. Rewind to start of this line.
                _pos = lineStart;
                _line = lineStartLine;
                _col = lineStartCol;
                break;
            }

            // For folded (>), a blank line preserved but adjacent non-blanks fold to space.
            // For literal (|), newlines preserved.
            if (!firstLine)
            {
                if (marker == '>' && !prevWasBlank)
                {
                    sb.Append(' ');
                }
                else if (marker == '|')
                {
                    sb.Append('\n');
                }
                else if (marker == '>' && prevWasBlank)
                {
                    // already appended newline; don't add space
                }
            }
            // Append rest of line content (from after blockIndent spaces)
            // We already advanced past leadingSpaces.
            while (_pos < _text.Length && _text[_pos] != '\n' && _text[_pos] != '\r')
            {
                sb.Append(_text[_pos]);
                Advance();
            }
            ConsumeNewline();
            firstLine = false;
            prevWasBlank = false;
        }

        // Trailing newline by convention for literal block scalars
        if (marker == '|' && sb.Length > 0 && sb[sb.Length - 1] != '\n')
            sb.Append('\n');

        return new YamlScalar(
            line,
            col,
            tag,
            sb.ToString(),
            marker == '|' ? ScalarStyle.Literal : ScalarStyle.Folded
        );
    }

    // -------- Tags --------

    private string ReadTag()
    {
        // _pos at '!'
        var sb = new StringBuilder();
        Advance();
        sb.Append('!');
        if (_pos < _text.Length && _text[_pos] == '!')
        {
            // !!set form
            sb.Append('!');
            Advance();
        }
        // Read tag chars (letters, digits, underscore, slash, period, colon, plus)
        while (_pos < _text.Length)
        {
            char c = _text[_pos];
            if (char.IsLetterOrDigit(c) || c == '_' || c == ':' || c == '.' || c == '/' || c == '+' || c == '-')
            {
                sb.Append(c);
                Advance();
            }
            else
                break;
        }
        return sb.ToString();
    }

    // -------- Flow constructs --------

    private YamlSequence ParseFlowSequence(string? tag, int line, int col)
    {
        // _pos at '['
        Advance();
        var items = new List<YamlNode>();
        while (true)
        {
            SkipFlowWhitespace();
            if (_pos >= _text.Length)
                throw Err("unterminated flow sequence");
            if (_text[_pos] == ']')
            {
                Advance();
                break;
            }
            items.Add(ParseFlowValue());
            SkipFlowWhitespace();
            if (_pos < _text.Length && _text[_pos] == ',')
            {
                Advance();
                continue;
            }
            if (_pos < _text.Length && _text[_pos] == ']')
            {
                Advance();
                break;
            }
            throw Err("expected ',' or ']' in flow sequence");
        }
        return new YamlSequence(line, col, tag, items);
    }

    private YamlMapping ParseFlowMapping(string? tag, int line, int col)
    {
        // _pos at '{'.
        // libminizinc spec only uses flow mappings as !!set bodies — members are
        // bare scalars treated as set elements (no values).
        Advance();
        var entries = new List<YamlMappingEntry>();
        while (true)
        {
            SkipFlowWhitespace();
            if (_pos >= _text.Length)
                throw Err("unterminated flow mapping");
            if (_text[_pos] == '}')
            {
                Advance();
                break;
            }
            int eLine = _line;
            int eCol = _col;
            var keyNode = ParseFlowValue();
            string keyText = keyNode is YamlScalar s ? s.Value : keyNode.ToString() ?? "";
            var keyScalar = new YamlScalar(eLine, eCol, null, keyText, ScalarStyle.Plain);
            SkipFlowWhitespace();
            YamlNode value;
            if (_pos < _text.Length && _text[_pos] == ':')
            {
                Advance();
                SkipFlowWhitespace();
                value = ParseFlowValue();
            }
            else
            {
                // bare member: value is the keyScalar itself
                value = keyScalar with { };
            }
            entries.Add(new YamlMappingEntry(keyScalar, value));
            SkipFlowWhitespace();
            if (_pos < _text.Length && _text[_pos] == ',')
            {
                Advance();
                continue;
            }
            if (_pos < _text.Length && _text[_pos] == '}')
            {
                Advance();
                break;
            }
            throw Err("expected ',' or '}' in flow mapping");
        }
        return new YamlMapping(line, col, tag, entries);
    }

    private YamlNode ParseFlowValue()
    {
        SkipFlowWhitespace();
        int line = _line;
        int col = _col;
        string? tag = null;
        if (_pos < _text.Length && _text[_pos] == '!')
        {
            tag = ReadTag();
            SkipFlowWhitespace();
        }
        if (_pos >= _text.Length)
            throw Err("unexpected end of flow value");
        char c = _text[_pos];
        if (c == '[')
            return ParseFlowSequence(tag, line, col);
        if (c == '{')
            return ParseFlowMapping(tag, line, col);
        if (c == '\'')
            return new YamlScalar(line, col, tag, ReadQuotedScalar('\''), ScalarStyle.SingleQuoted);
        if (c == '"')
            return new YamlScalar(line, col, tag, ReadQuotedScalar('"'), ScalarStyle.DoubleQuoted);

        // Plain flow scalar: until ',' or ']' or '}' or EOL
        var sb = new StringBuilder();
        while (_pos < _text.Length)
        {
            char ch = _text[_pos];
            if (ch == ',' || ch == ']' || ch == '}' || ch == '\n' || ch == '\r')
                break;
            sb.Append(ch);
            Advance();
        }
        return new YamlScalar(line, col, tag, sb.ToString().Trim(), ScalarStyle.Plain);
    }

    private void SkipFlowWhitespace()
    {
        while (_pos < _text.Length)
        {
            char c = _text[_pos];
            if (c == ' ' || c == '\t')
            {
                Advance();
                continue;
            }
            if (c == '\n' || c == '\r')
            {
                ConsumeNewline();
                continue;
            }
            if (c == '#')
            {
                while (_pos < _text.Length && _text[_pos] != '\n' && _text[_pos] != '\r')
                    Advance();
                continue;
            }
            break;
        }
    }

    // -------- Errors --------

    private YamlParseException Err(string message) =>
        new YamlParseException(message, _line, _col, _path);
}
