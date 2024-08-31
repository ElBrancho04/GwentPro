using System.Diagnostics;

class Lexer
{
    readonly string Source;
    int Line;
    int Pos;
    public Lexer(string source)
    {
        Source = source;
        Line = 0;
        Pos = 0;
    }

    readonly string[] keywords = {"effect","card", "Name","Params","Action","Type",
    "Faction","Power","Range","OnActivation","Effect","Selector","Source","Single","Predicate","PostAction",
    "if","else","for","while", "true", "false", "in", "Number", "String", "Bool"};

    public List<Token> Tokenize()
    {
        List<Token> tokens = new List<Token>();
        bool skip = false;
        for (int i = 0; i <= Source.Length; i++)
        {   
            Pos++;
            if (Source[i] == '\n')
            {
                Line++;
                Pos = 0;
            }
            if (skip) {skip = false; continue;}
            if (i == Source.Length)
            tokens.Add(new Token(TokenType.EOF, "#", Line, Pos));

            else 
            switch (Source[i])
            {
                // Operadores aritméticos
                case '+':
                if (i+1<Source.Length && Source[i+1] == '+')
                {
                    tokens.Add(new Token(TokenType.AritOperator, "++", Line, Pos));
                    skip = true;
                    continue;
                }
                tokens.Add(new Token(TokenType.AritOperator,"+", Line, Pos));
                break;

                case '-':
                if (i+1<Source.Length && Source[i+1] == '-')
                {
                    tokens.Add(new Token(TokenType.AritOperator, "--", Line, Pos));
                    skip = true;
                    continue;
                }
                tokens.Add(new Token(TokenType.AritOperator,"-", Line, Pos));
                break;

                case '*':
                tokens.Add(new Token(TokenType.AritOperator,"*", Line, Pos));
                break;

                case '/':
                tokens.Add(new Token(TokenType.AritOperator,"/", Line, Pos));
                break;

                // Concatenación de cadenas
                case '@':
                if (i+1<Source.Length && Source[i+1] == '@')
                {
                    tokens.Add(new Token(TokenType.AritOperator, "@@", Line, Pos));
                    skip = true;
                    continue;
                }
                tokens.Add(new Token(TokenType.AritOperator,"@", Line, Pos));
                break;

                // Operadores lógicos
                case '&':
                if (i+1<Source.Length && Source[i+1] == '&')
                {
                    tokens.Add(new Token(TokenType.LogOperator,"&&", Line, Pos));
                    skip = true;
                    continue;
                }
                break;

                case '|':
                if (i+1<Source.Length && Source[i+1] == '|')
                {
                    tokens.Add(new Token(TokenType.LogOperator,"||", Line, Pos));
                    skip = true;
                    continue;
                }
                break;

                // Comparadores
                case '<':
                if (i+1<Source.Length && Source[i+1] == '=')
                {
                    tokens.Add(new Token(TokenType.Comparator, "<=", Line, Pos));
                    skip = true;
                    continue;
                }
                tokens.Add(new Token(TokenType.Comparator,"<", Line, Pos));
                break;

                case '>':
                if (i+1<Source.Length && Source[i+1] == '=')
                {
                    tokens.Add(new Token(TokenType.Comparator, ">=", Line, Pos));
                    skip = true;
                    continue;
                }
                tokens.Add(new Token(TokenType.Comparator,">", Line, Pos));
                break;

                // Asignación o comparador "=="
                case '=':
                if (i+1<Source.Length && Source[i+1] == '=')
                {
                    tokens.Add(new Token(TokenType.Comparator, "==", Line, Pos));
                    skip = true;
                    continue;
                }
                if (i+1<Source.Length && Source[i+1] == '>')
                {
                    tokens.Add(new Token(TokenType.Punctuation, "=>", Line, Pos));
                    skip = true;
                    continue;
                }
                tokens.Add(new Token(TokenType.Assigment, "=", Line, Pos));
                break;

                // Separadores
                case '(':
                tokens.Add(new Token(TokenType.Punctuation, "(", Line, Pos));
                break;

                case ')':
                tokens.Add(new Token(TokenType.Punctuation, ")", Line, Pos));
                break;

                case '{':
                tokens.Add(new Token(TokenType.Punctuation, "{", Line, Pos));
                break;

                case '}':
                tokens.Add(new Token(TokenType.Punctuation, "}", Line, Pos));
                break;

                case '[':
                tokens.Add(new Token(TokenType.Punctuation, "[", Line, Pos));
                break;

                case ']':
                tokens.Add(new Token(TokenType.Punctuation, "]", Line, Pos));
                break;

                // Signos de puntuación
                case ',':
                tokens.Add(new Token(TokenType.Punctuation, ",", Line, Pos));
                break;

                case ';':
                tokens.Add(new Token(TokenType.Punctuation, ";", Line, Pos));
                break;

                case '.':
                tokens.Add(new Token(TokenType.Punctuation, ".", Line, Pos));
                break;

                case ':':
                tokens.Add(new Token(TokenType.Punctuation, ":", Line, Pos));
                break;

                case '!':
                if (i+1<Source.Length && Source[i+1] == '=')
                {
                    tokens.Add(new Token(TokenType.Comparator, "!=", Line, Pos));
                    skip = true;
                    continue;
                }
                tokens.Add(new Token(TokenType.LogOperator, "!", Line, Pos));
                break; 

                // Procesando strings
                case '"':
                int aux = Pos;
                string str = "";
                for (int j = i+1; j < Source.Length; j++)
                {
                    aux++;
                    if (Source[j] == '\n')
                    {
                        aux=0;
                        Line++;
                    }
                    if (Source[j] == '"')
                    {
                        tokens.Add(new Token(TokenType.String, str, Line, Pos));
                        i = j;
                        Pos = aux;
                        continue;
                    }
                    str += Source[j];
                }
                break;

                default:
                if (char.IsDigit(Source[i]))
                {
                    int auxPos = Pos;
                    string stri = "";
                    stri += Source[i];
                    for (int j = i+1; j < Source.Length; j++)
                    {
                        auxPos++;
                        if (j>=Source.Length) break;
                        if (!char.IsDigit(Source[j]))
                        {
                            tokens.Add(new Token(TokenType.Number, int.Parse(stri), Line, Pos));
                            Pos = auxPos-1;
                            i = j-1;
                            break;
                        }
                        stri += Source[j];
                    }
                    break;
                }
                else if (char.IsLetter(Source[i]))
                {
                    int auxPos = Pos;
                    string stri = "";
                    stri += Source[i];
                    for (int j = i+1; j < Source.Length; j++)
                    {
                        auxPos++;
                        if (j>=Source.Length) break;
                        if (!(char.IsLetterOrDigit(Source[j]) || Source[j] == '_'))
                        {
                            if (keywords.Contains(stri))
                            tokens.Add(new Token(TokenType.Kewyword, stri, Line, Pos));
                            else
                            tokens.Add(new Token(TokenType.Identifier, stri, Line, Pos));
                            Pos = auxPos-1;
                            i = j-1;
                            break;
                        }
                        stri += Source[j];
                    }
                }
                break;
            }
        }
        return tokens;
    }
}