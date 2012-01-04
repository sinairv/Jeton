using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;

namespace Jeton
{
    public class Java2CsSyntaxConverter
    {
        private readonly List<KeyValuePair<string, string>> m_replacements = new List<KeyValuePair<string, string>>();
        private readonly List<KeyValuePair<string, string>> m_customReplacements = new List<KeyValuePair<string, string>>();
        private readonly List<KeyValuePair<string, string>> m_regexReplacements = new List<KeyValuePair<string, string>>();

        public Java2CsSyntaxConverter()
        {
            InitReplacements();
        }

        private void InitReplacements()
        {
            AddTokenReplacemen("package", "namespace"); // token
            AddTokenReplacemen("import", "using"); // token
            AddTokenReplacemen("extends", ":"); // token
            AddTokenReplacemen("implements", ":"); // token
            AddRegexReplacemen(@"\bArrayList\<", "List<"); // generic-class-name
            AddTokenReplacemen("final", "const"); // token
            AddTokenReplacemen("boolean", "bool"); // token
            AddTokenReplacemen("Object", "object"); // token
            AddTokenReplacemen("super", "base"); // token

            AddRegexReplacemen(@"\bconst\s+class\b", "class"); // multi-tokens
            AddRegexReplacemen(@"\bstatic\s+\bconst\b", "const"); // multi-tokens
            AddTokenReplacemen("String", "string"); // token
            AddRegexReplacemen(@"\(\s+const\b", "("); // token in args
            AddRegexReplacemen(@",\s+const\b", ", "); // token in args
            AddRegexReplacemen(@"\bSystem\s*\.\s*out\s*.\s*println\b", "Console.WriteLine"); // expression
            AddRegexReplacemen(@"\bSystem\s*\.\s*out\s*.\s*printf\b", "Console.Write"); // expression
            AddRegexReplacemen(@"\bSystem\s*\.\s*out\s*.\s*print\b", "Console.Write"); // expression
            AddTokenReplacemen(@"println", "WriteLine"); //token
            AddTokenReplacemen(@"print", "Write"); // token

            AddTokenReplacemen("instanceof", "is"); // token
            //AddTokenReplacemen(@"replaceAll", "Replace");
            AddRegexReplacemen(@"\bsize\s*\(\s*\)", "Count"); // argument-less mthod
            AddRegexReplacemen(@"\b\(Locale\s*.\s*ENGLISH\s*,", "("); // ?
            AddRegexReplacemen(@"\bSystem\s*.\s*currentTimeMillis\s*\(\s*\)", "DateTime.Now.Ticks * 10000"); // argument-less mthod
            AddTokenReplacemen("NullPointerException", "NullReferenceException"); // token
            AddTokenReplacemen("IllegalArgumentException", "ArgumentException"); // token
            AddTokenReplacemen("IllegalStateException", "Exception"); // token
            AddTokenReplacemen("trim", "Trim"); // token
            AddTokenReplacemen("toString", "ToString"); // token
            AddTokenReplacemen("InputStream", "StreamReader"); // token
            AddTokenReplacemen("equals", "Equals"); // token
            AddTokenReplacemen("lastIndexOf", "LastIndexOf"); // token
            AddTokenReplacemen("URI", "Uri"); // token
            AddTokenReplacemen("URL", "Url"); // token
            AddTokenReplacemen("exists", "Exists"); // token
            AddTokenReplacemen("endsWith", "EndsWith"); // token

            AddTokenReplacemen("toUpperCase", "ToUpper"); // token
            AddTokenReplacemen("toLowerCase", "ToLower"); // token
            AddTokenReplacemen("substring", "SubstringWithIndex"); // ?
            AddTokenReplacemen("Character", "Char"); // token
            AddTokenReplacemen("isUpperCase", "IsUpper"); // token
            AddTokenReplacemen("isLowerCase", "IsLower"); // token
            AddTokenReplacemen("isLetter", "IsLetter"); // token
            AddTokenReplacemen("isLetterOrDigit", "IsLetterOrDigit"); // token
            AddTokenReplacemen("append", "Append"); // token
            AddTokenReplacemen("replace", "Replace"); // token
            AddTokenReplacemen("indexOf", "IndexOf"); // token
            AddRegexReplacemen(@"\.\s*length\s*\(\s*\)", ".Length"); // argument-less method
            AddRegexReplacemen(@"\.\s*length\b", ".Length"); // field

            AddRegexReplacemen(@"\.\s*getMessage\s*\(\s*\)", ".Message"); // argument-less method

            AddTokenReplacemen("add", "Add"); // token
            AddTokenReplacemen("containsKey", "ContainsKey"); // token
            AddTokenReplacemen("put", "Add"); // token
            AddTokenReplacemen("Arrays", "Array"); // token
            AddTokenReplacemen("remove", "Remove"); // token
            AddTokenReplacemen("startsWith", "StartsWith"); // token
            AddTokenReplacemen("split", "Split"); // token
            AddTokenReplacemen("readLine", "ReadLine"); // token
            AddTokenReplacemen("isDigit", "IsDigit"); // token
            AddTokenReplacemen("isWhitespace", "IsWhiteSpace"); // token

            AddTokenReplacemen("sort", "Sort"); // token
            AddTokenReplacemen("peek", "Peek"); // token
            AddTokenReplacemen("offer", "Enqueue"); // token
            AddTokenReplacemen("poll", "Dequeue"); // token

            AddTokenReplacemen("close", "Close"); // token
            AddTokenReplacemen("Class", "Type"); // token
            AddTokenReplacemen("Set", "HashSet"); // token
            AddTokenReplacemen("TreeSet", "HashSet"); // token
            AddTokenReplacemen("HashMap", "Dictionary"); // token
            AddTokenReplacemen("Map", "Dictionary"); // token
            AddTokenReplacemen("ArrayBlockingQueue", "Queue"); // token
            AddTokenReplacemen("Collection", "ICollection"); // token
            AddTokenReplacemen("Comparable", "IComparable"); // token
            AddTokenReplacemen("clear", "Clear"); // token
            AddTokenReplacemen("PrintStream", "TextWriter"); // token

            AddTokenReplacemen("InputStreamReader", "StreamReader"); // token
            AddTokenReplacemen("BufferedInputStream", "BufferedStream"); // token
            AddTokenReplacemen("BufferedReader", "BufferedStream"); // token
            AddTokenReplacemen("Enumeration", "IEnumerator"); // token
            AddTokenReplacemen("Iterator", "IEnumerator"); // token
            AddRegexReplacemen(@"\biterator\(\s*\)", "GetEnumerator()"); // argument-less method
            AddRegexReplacemen(@"\bhasNext\(\s*\)", "MoveNext()"); // argument-less method
            AddRegexReplacemen(@"\bnext\(\s*\)", "Current"); // argument-less method
            AddTokenReplacemen("hasMoreElements", "MoveNext"); // token
            AddRegexReplacemen(@"\bnextElement\s*\(\s*\)", "Current"); // argument-less method

            AddTokenReplacemen("toArray", "ToArray"); // token
            AddTokenReplacemen("clone", "Clone"); // token
            AddTokenReplacemen("hashCode", "GetHashCode"); // token
            AddTokenReplacemen("getClass", "GetType"); // token
            AddTokenReplacemen("contains", "Contains"); // token
            AddTokenReplacemen("CharSequence", "string"); // token
            AddTokenReplacemen("Long", "Int64"); // token
            AddTokenReplacemen("Double", "double"); // token
            AddTokenReplacemen("Integer", "int"); // token
            AddTokenReplacemen("parseInt", "Parse"); // token
            AddTokenReplacemen("parseDouble", "Parse"); // token
            AddTokenReplacemen("parseLong", "Parse"); // token
            AddTokenReplacemen("min", "Min"); // token
            AddTokenReplacemen("max", "Max"); // token

            AddTokenReplacemen("EmptyStackException", "Exception"); // token
            AddTokenReplacemen("RuntimeException", "Exception"); // token
            AddTokenReplacemen("ClassNotFoundException", "Exception"); // token
            AddTokenReplacemen("MissingResourceException", "Exception"); // token
            AddTokenReplacemen("ClassCastException", "Exception"); // token
            AddTokenReplacemen("Throwable", "Exception"); // token
            
            AddRegexReplacemen(@"\bstatic\s+enum\b", "enum"); // mutli-token


            AddRegexReplacemen(@"\bSystem\s*\.\s*err\b", "Console.Error"); // expression
            AddRegexReplacemen(@"\bSystem\s*\.\s*out\b", "Console.Out"); // expression

            AddRegexReplacemen(@"\bjava\s*\.\s*lang\b", "System"); // expression

            AddRegexReplacemen(@"\bSystem\s*\.\s*arraycopy\b", "Array.Copy"); // expression

            AddTokenReplacemen("Locale", "CultureInfo"); // token
            AddTokenReplacemen("Reader", "TextReader"); // token

            AddTokenReplacemen("compareTo", "CompareTo"); // token

            AddCustomReplacemen("de.danielnaber.", ""); // ?
            AddCustomReplacemen("languagetool.tools", "SharpLanguageTool.Tools"); // ?
            AddCustomReplacemen("isEmpty", "IsEmpty"); // token
            // mot good for all applications
            //AddCustomReplacemen("const ", "");
            AddRegexReplacemen(@"\bsb\s*.\s*append\b", "sb.Append"); // expression

            AddRegexReplacemen(@"\bthrows\s+\b\w+\b", ""); // regexp
            AddRegexReplacemen(@"\bstr\s*\.\s*length\s*\(\s*\)", "str.Length"); // regexp

            AddRegexReplacemen(@"\bthis\s+\=\=\s+obj\b", "Object.ReferenceEquals(this, obj)"); // regexp

            AddRegexReplacemen(@"\bArray\s*\.\s*ToString\b", "ArrayUtils.ToString"); // expression + extension methods
        }

        public void AddTokenReplacemen(string key, string value)
        {
            m_regexReplacements.Add(new KeyValuePair<string, string>(@"\b" + key + @"\b", value));
        }


        public void AddReplacemen(string key, string value)
        {
            m_replacements.Add(new KeyValuePair<string, string>(key, value));
        }

        public void AddCustomReplacemen(string key, string value)
        {
            m_customReplacements.Add(new KeyValuePair<string, string>(key, value));
        }

        public void AddRegexReplacemen(string key, string value)
        {
            m_regexReplacements.Add(new KeyValuePair<string, string>(key, value));
        }

        public string Convert(string allInput)
        {
            allInput = PerformRegexReplacements(allInput);
            allInput = PerformReplacements(allInput);
            allInput = PerformCustomReplacements(allInput);

            var sb = new StringBuilder();

            var sr = new StringReader(allInput);

            bool isNamespaceBegun = false;
            bool isInsideSpecialComment = false;
            bool isInOverride = false;

            string rawline;
            while ((rawline = sr.ReadLine()) != null)
            {
                bool removeLine = false;
                rawline = rawline.Replace("\\p{Punct}", "\\p{P}");
                string line = rawline.Trim();
                string postLines = "";
                string preLines = "";

                if(line.StartsWith("@Override"))
                {
                    isInOverride = true;
                    removeLine = true;
                }
                else if(isInOverride)
                {
                    if(!String.IsNullOrEmpty(line))
                    {
                        int idx = FirstNonWhitespaceIndex(rawline);
                        rawline = rawline.Insert(idx, "override ");
                        isInOverride = false;
                    }
                }

                if (line.StartsWith("namespace"))
                {
                    rawline = rawline.Replace(";", "");
                    isNamespaceBegun = true;
                    postLines = "{";
                    preLines = @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SharpLanguageTool.Extensions;
";
                }
                else if (line.StartsWith("using"))
                {
                    removeLine = true;
                }
                else if (line.Contains("serialVersionUID") && line.Contains("private") && line.EndsWith(";"))
                {
                    removeLine = true;
                }


                if (line.StartsWith("/**"))
                {
                    isInsideSpecialComment = true;
                    rawline = rawline.Replace("/**", "/// <summary>");
                }
                else if (line.StartsWith("*/") && isInsideSpecialComment)
                {
                    rawline = rawline.Replace(" */", "/// </summary>");
                    isInsideSpecialComment = false;
                }
                else if (line.StartsWith("*") && isInsideSpecialComment)
                {
                    rawline = rawline.Replace(" *", "///");
                }
                else
                {
                    isInsideSpecialComment = false;
                }

                var matchConst = Regex.Match(rawline, @"\bconst\b");
                if(matchConst.Success)
                {
                    // do not convert for field definitions
                    if (!(rawline.EndsWith(";") && FirstNonWhitespaceIndex(rawline) == 4))
                    {
                        rawline = Regex.Replace(rawline, @"\bconst\b\s*", "");
                    }
                }

                if (line.StartsWith("for"))
                {
                    if (line.Contains(":"))
                    {
                        rawline = rawline.Replace("for", "foreach").Replace(":", "in");
                    }
                }

                var matchToArray = Regex.Match(rawline, @"\bToArray\s*\(");
                if (matchToArray.Success)
                {
                    // empty its argument
                    int ops = 1;
                    int idx = matchToArray.Index + matchToArray.Length;

                    int argLength = -1;
                    for (int i = idx; i < rawline.Length; i++)
                    {
                        if (rawline[i] == '(')
                            ops++;
                        else if (rawline[i] == ')')
                            ops--;

                        if (ops == 0)
                        {
                            argLength = i - idx;
                            break;
                        }
                    }

                    if (argLength > 0)
                    {
                        var sbNewLine = new StringBuilder(rawline);
                        sbNewLine.Remove(idx, argLength);
                        rawline = sbNewLine.ToString();
                    }
                }

                var matchGet = Regex.Match(rawline, @"\.\s*get\s*\(");
                if (matchGet.Success)
                {
                    // change it to indexer

                    // index of open par
                    int idx = matchGet.Index + matchGet.Length - 1;

                    // first change pars to brackets then remove .get
                    var sbNewLine = new StringBuilder(rawline);

                    Debug.Assert(sbNewLine[idx] == '(');
                    sbNewLine[idx] = '[';

                    int ops = 1;
                    for (int i = idx + 1; i < sbNewLine.Length; i++)
                    {
                        if (sbNewLine[i] == '(')
                            ops++;
                        else if (sbNewLine[i] == ')')
                            ops--;

                        if (ops == 0)
                        {
                            sbNewLine[i] = ']';
                            break;
                        }
                    }

                    sbNewLine.Remove(matchGet.Index, matchGet.Length - 1);
                    //rawline.

                    rawline = sbNewLine.ToString();

                }

                var matchCharAt = Regex.Match(rawline, @"\.\s*charAt\b");
                if(matchCharAt.Success)
                {
                    int idx = matchCharAt.Index;

                    var sbNewLine = new StringBuilder(rawline);
                    sbNewLine.Remove(idx, matchCharAt.Length);

                    int ops = 1;
                    idx = sbNewLine.ToString().IndexOf('(', idx);
                    sbNewLine[idx] = '[';
                    for (int i = idx + 1; i < sbNewLine.Length; i++)
                    {
                        if (sbNewLine[i] == '(')
                            ops++;
                        else if (sbNewLine[i] == ')')
                            ops--;

                        if (ops == 0)
                        {
                            sbNewLine[i] = ']';
                            break;
                        }
                    }
                    //rawline.

                    rawline = sbNewLine.ToString();
                }


                if (line.Contains("\""))
                {
                    int firstIndex = rawline.IndexOf('\"');
                    int lastIndex = rawline.LastIndexOf('\"');

                    if (firstIndex >= 0 && lastIndex >= 0)
                    {
                        string inBetween = rawline.Substring(firstIndex, lastIndex - firstIndex + 1);

                        if (inBetween.Contains('%'))
                        {
                            int formatCount = 0;

                            string prior = rawline.Substring(0, firstIndex);
                            string posterior = "";
                            if (lastIndex + 1 < rawline.Length)
                                posterior = rawline.Substring(lastIndex + 1);

                            var newInBetween = new StringBuilder();

                            bool formatStarted = false;
                            int i = 0;
                            string curFormat = "";
                            while (i < inBetween.Length)
                            {
                                if (inBetween[i] == '%')
                                {
                                    if (i + 1 < inBetween.Length && inBetween[i + 1] == '%')
                                    {
                                        newInBetween.Append("%%");
                                        i++; // with the final inc will result in i += 2
                                    }
                                    else
                                    {
                                        formatStarted = true;
                                        curFormat = "";
                                    }
                                }
                                else if (formatStarted && Char.IsLetter(inBetween[i]))
                                {
                                    formatStarted = false;

                                    int beforeDot = -1;
                                    int afterDot = -1;
                                    int tmpResult;
                                    int dotIndex = curFormat.IndexOf('.');
                                    if (dotIndex >= 0)
                                    {
                                        if (Int32.TryParse(curFormat.Substring(0, dotIndex), out tmpResult))
                                            beforeDot = tmpResult;

                                        curFormat = dotIndex + 1 < curFormat.Length ? curFormat.Substring(dotIndex + 1) : "";
                                    }

                                    if (Int32.TryParse(curFormat, out tmpResult))
                                        afterDot = tmpResult;

                                    string newFormat = "{" + formatCount.ToString();

                                    if (beforeDot >= 0)
                                    {
                                        newFormat += "," + beforeDot.ToString();
                                    }

                                    if (afterDot >= 0)
                                    {
                                        newFormat += ":F" + afterDot.ToString();
                                    }

                                    newFormat += "}";

                                    newInBetween.Append(newFormat);
                                    formatCount++;

                                    // TODO
                                }
                                else if (formatStarted)
                                {
                                    curFormat += inBetween[i];
                                }
                                else
                                {
                                    newInBetween.Append(inBetween[i]);
                                }


                                i++;
                            }


                            rawline = prior + newInBetween + posterior;
                        }
                    }
                }

                // now insert
                if (!removeLine)
                {
                    if (!String.IsNullOrEmpty(preLines))
                    {
                        sb.AppendLine(preLines);
                    }
                    sb.AppendLine(rawline);
                    if (!String.IsNullOrEmpty(postLines))
                    {
                        sb.AppendLine(postLines);
                    }
                }

            }

            if (isNamespaceBegun)
            {
                sb.AppendLine("}");
            }

            return sb.ToString();
        }

        private int FirstNonWhitespaceIndex(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (!Char.IsWhiteSpace(str[i]))
                {
                    return i;
                }
            }

            return str.Length;
        }

        private string PerformReplacements(string allInput)
        {
            foreach (var pair in m_replacements)
            {
                allInput = allInput.Replace(pair.Key, pair.Value);
            }

            return allInput;

        }

        private string PerformCustomReplacements(string allInput)
        {
            foreach (var pair in m_customReplacements)
            {
                allInput = allInput.Replace(pair.Key, pair.Value);
            }

            return allInput;
        }

        private string PerformRegexReplacements(string allInput)
        {
            foreach (var pair in m_regexReplacements)
            {
                string pattern = pair.Key;
                string replace = pair.Value;
                var r = new Regex(pattern);
                allInput = r.Replace(allInput, replace);
            }
            return allInput;
        }

    }
}
