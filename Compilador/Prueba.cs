if (currTokenVal() != "}")
        {
            if (currTokenVal() != ",")
            {
                ReportError("Token inesperado en la declaración de effect");
                return nameNode;
            }
            else
            {
                if (Peek().Value is string && (string)Peek().Value == "}")
                {
                    ReportError("La coma es innecesaria");
                }
                Advance();
                return nameNode;
            }
        }
        else return nameNode;