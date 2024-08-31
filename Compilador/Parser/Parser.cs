public class Parser
{
    private List<Token> tokens;
    private List<Error> errors;
    private int pos;
    private Token currentToken;
    private string currTokenVal()
    {
        return (currentToken.Value is string) ? currTokenVal() : "";
    }

    public Parser(List<Token> tokenss)
    {
        tokens = tokenss;
        pos = 0;
        currentToken = tokens[pos];
        errors = new List<Error>();
    }

    private void Advance()
    {
        pos++;
        if (pos < tokens.Count - 1)
        {
            currentToken = tokens[pos];
        }
        else if (errors.Count > 0)
        {
            foreach (var item in errors)
            {
                System.Console.WriteLine(item);
            }
        }
        else
        {
            // !!!!!!! Aquí va algo !!!!!!
        }
    }

    private Token Peek(int tokensAhead = 1)
    {
        if (pos + tokensAhead >= tokens.Count || pos + tokensAhead < 0)
        {
            throw new Exception("La posicion pedida está fuera del rango");
        }
        return tokens[pos + tokensAhead];
    }

    private void ReportError(string message)
    {
        errors.Add(new Error(currentToken.Line, currentToken.Pos, message));
    }

    public ASTNode Parse()
    {
        return ParseProgram();
    }

    private ASTNode ParseProgram()
    {
        var programNode = new MultiChildNode(NodeType.Program, "Program", 0, 0);

        while (currentToken.TokenType != TokenType.EOF)
        {
            if (currTokenVal() == "card")
            {
                var cardNode = ParseCardDecl();
                programNode.AddChild(cardNode);
            }
            else if (currTokenVal() == "effect")
            {
                var effectNode = ParseEffectDecl();
                programNode.AddChild(effectNode);
            }
            else
            {
                ReportError($"Token inesperado: {currentToken.Value}, sólo se permite declarar card y effect en este contexto");
                while (currTokenVal() != "card" && currTokenVal() != "effect")
                    Advance();

                continue;
            }
        }

        return programNode;
    }

    private ASTNode ParseEffectDecl()
    {
        var effectNode = new MultiChildNode(NodeType.EfectDecl, "effect", currentToken.Line, currentToken.Pos);
        Advance();
        if (currTokenVal() != "{")
        {
            ReportError("Token inesperado, se esperaba un {");
            while (currTokenVal() != "card" && currTokenVal() != "effect")
                Advance();
            return effectNode;
        }
        Advance();
        while (currentToken.TokenType != TokenType.EOF && currTokenVal() != "}")
        {
            if (currTokenVal() == "card" || currTokenVal() == "effect")
            {
                ReportError("Declaración de efecto incompleta, se esperaba: }");
                return effectNode;
            }
            if (currTokenVal() == "Name")
            {
                var NameNode = ParseEfctDeclName();
                effectNode.AddChild(NameNode);
            }
            if (currTokenVal() == "Params")
            {
                var ParamsNode = ParseEfctDeclParams();
                effectNode.AddChild(ParamsNode);
            }
            if (currTokenVal() == "Action")
            {
                var ActionNode = ParseEfctDeclAction();
                effectNode.AddChild(ActionNode);
            }
        }
        if (currTokenVal() == "}") Advance();
        else ReportError("Debe terminar la declaración de effect con }");
        return effectNode;
    }

    private ASTNode ParseEfctDeclName()
    {
        var nameNode = new UnaryNode(NodeType.Property, "Name", currentToken.Line, currentToken.Pos);
        Advance();
        if (currTokenVal() != ":")
        {
            ReportError("Token inesperado, se esperaba ':'");
            while (currTokenVal() != "Name" && currTokenVal() != "Params" && currTokenVal() != "Action" && currTokenVal() != "card" && currTokenVal() != "effect")
            {
                Advance();
            }
            return nameNode;
        }
        Advance();
        nameNode.Child = ParseExpression();
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
    }
    private ASTNode ParseEfctDeclParams()
    {
        var paramsNode = new MultiChildNode(NodeType.Property, "Params", currentToken.Line, currentToken.Pos);
        Advance();
        if (currTokenVal() != ":")
        {
            ReportError("Token inesperado, se esperaba ':'");
            while (currTokenVal() != "Name" && currTokenVal() != "Params" && currTokenVal() != "Action" && currTokenVal() != "card" && currTokenVal() != "effect")
            {
                Advance();
            }
            return paramsNode;
        }
        Advance();
        if (currTokenVal() != "{")
        {
            ReportError("Token inesperado, se esperaba '{'");
            while (currTokenVal() != "Name" && currTokenVal() != "Params" && currTokenVal() != "Action" && currTokenVal() != "card" && currTokenVal() != "effect")
            {
                Advance();
            }
            return paramsNode;
        }
        Advance();
        while (currentToken.TokenType == TokenType.Identifier)
        {
            var paramIDNode = new ASTNode(NodeType.Identifier, currentToken.Line, currentToken.Pos, currentToken.Value);
            var paramNode = new BinaryNode(NodeType.Property, "Param", currentToken.Line, currentToken.Pos, paramIDNode);
            Advance();
            if (currTokenVal() != ":")
            {
                ReportError("Token inesperado, se esperaba ':'");
                while (currTokenVal() != "Name" && currTokenVal() != "Params" && currTokenVal() != "Action" && currTokenVal() != "card" && currTokenVal() != "effect")
                {
                    if (currTokenVal() == ",")
                    {
                        if (Peek().Value is string && (string)Peek().Value == "}")
                        {
                            ReportError("La coma es innecesaria");
                            Advance();
                            Advance();
                            return paramsNode;
                        }
                        Advance();
                        break;
                    }
                    Advance();
                }
                return paramsNode;
            }
            Advance();
            if (currTokenVal() != "Number" && currTokenVal() != "String" && currTokenVal() != "Bool")
            {
                ReportError("Token inesperado, sólo se aceptan: Number, String o Bool");
                while (currTokenVal() != "Name" && currTokenVal() != "Params" && currTokenVal() != "Action" && currTokenVal() != "card" && currTokenVal() != "effect")
                {
                    if (currTokenVal() == ",")
                    {
                        if (Peek().Value is string && (string)Peek().Value == "}")
                        {
                            ReportError("La coma es innecesaria");
                            Advance();
                            Advance();
                            return paramsNode;
                        }
                        Advance();
                        break;
                    }
                    Advance();
                }
                return paramsNode;
            }
            paramNode.Right = new ASTNode(NodeType.Literal, currentToken.Line, currentToken.Pos, currentToken.Value);
            paramsNode.AddChild(paramNode);
            Advance();
            if (currTokenVal() == ",")
            {
                if (Peek().Value is string && (string)Peek().Value == "}")
                {
                    ReportError("La coma es innecesaria");
                    Advance();
                    Advance();
                    return paramsNode;
                }
                Advance();
            }
        }
        if (currTokenVal() != "}")
        {
            ReportError("Token inesperado, se debe concluir la declaración con un '}'");
        }
        else Advance();
        if (currTokenVal() != "}")
        {
            if (currTokenVal() != ",")
            {
                ReportError("Token inesperado en la declaración de effect");
                return paramsNode;
            }
            else
            {
                if (Peek().Value is string && (string)Peek().Value == "}")
                {
                    ReportError("La coma es innecesaria");
                }
                Advance();
                return paramsNode;
            }
        }
        else return paramsNode;
    }
    private ASTNode ParseEfctDeclAction()
    {
        var actionNode = new BinaryNode(NodeType.Property, "Action", currentToken.Line, currentToken.Pos);
        Advance();
        if (currTokenVal() != ":")
        {
            ReportError("Token inesperado, se esperaba ':'");
            while (currTokenVal() != "Name" && currTokenVal() != "Params" && currTokenVal() != "Action" && currTokenVal() != "card" && currTokenVal() != "effect")
            {
                Advance();
            }
            return actionNode;
        }
        Advance();
        if (currTokenVal() != "(")
        {
            ReportError("Token inesperado, se esperaba '('");
            while (currTokenVal() != "Name" && currTokenVal() != "Params" && currTokenVal() != "Action" && currTokenVal() != "card" && currTokenVal() != "effect")
            {
                if (currTokenVal() == "{")
                {
                    Advance();
                    actionNode.Right = ParseBlock();
                    if (currTokenVal() != "}")
                        ReportError("Token inesperado, debe terminar la declaración con '}'");
                    else
                        Advance();
                    return actionNode;
                }
                Advance();
            }
            return actionNode;
        }
        Advance();
        if (currentToken.TokenType != TokenType.Identifier)
        {
            ReportError("Token inesperado, se esperaba un identificador");
            while (currTokenVal() != "Name" && currTokenVal() != "Params" && currTokenVal() != "Action" && currTokenVal() != "card" && currTokenVal() != "effect")
            {
                if (currTokenVal() == "{")
                {
                    Advance();
                    actionNode.Right = ParseBlock();
                    if (currTokenVal() != "}")
                        ReportError("Token inesperado, debe terminar la declaración con '}'");
                    else
                        Advance();
                    return actionNode;
                }
                Advance();
            }
            return actionNode;
        }
        var paramsNode = new BinaryNode(NodeType.EfctDeclActionParms, "Params", currentToken.Line, currentToken.Pos, new ASTNode(NodeType.Identifier, currentToken.Line, currentToken.Pos, currentToken.Value));
        actionNode.Left = paramsNode;
        Advance();
        if (currTokenVal() != ",")
        {
            ReportError("Token inesperado, se esperaba ','");
            while (currTokenVal() != "Name" && currTokenVal() != "Params" && currTokenVal() != "Action" && currTokenVal() != "card" && currTokenVal() != "effect")
            {
                if (currTokenVal() == "{")
                {
                    Advance();
                    actionNode.Right = ParseBlock();
                    if (currTokenVal() != "}")
                        ReportError("Token inesperado, debe terminar la declaración con '}'");
                    else
                        Advance();
                    return actionNode;
                }
                Advance();
            }
            return actionNode;
        }
        Advance();
        if (currentToken.TokenType != TokenType.Identifier)
        {
            ReportError("Token inesperado, se esperaba un identificador");
            while (currTokenVal() != "Name" && currTokenVal() != "Params" && currTokenVal() != "Action" && currTokenVal() != "card" && currTokenVal() != "effect")
            {
                if (currTokenVal() == "{")
                {
                    Advance();
                    actionNode.Right = ParseBlock();
                    if (currTokenVal() != "}")
                        ReportError("Token inesperado, debe terminar la declaración con '}'");
                    else
                        Advance();
                    return actionNode;
                }
                Advance();
            }
            return actionNode;
        }
        paramsNode.Right = new ASTNode(NodeType.Identifier, currentToken.Line, currentToken.Pos, currentToken.Value);
        Advance();
        if (currTokenVal() != ")")
        {
            ReportError("Token inesperado, se esperaba ')'");
            while (currTokenVal() != "Name" && currTokenVal() != "Params" && currTokenVal() != "Action" && currTokenVal() != "card" && currTokenVal() != "effect")
            {
                if (currTokenVal() == "{")
                {
                    Advance();
                    actionNode.Right = ParseBlock();
                    if (currTokenVal() != "}")
                        ReportError("Token inesperado, debe terminar la declaración con '}'");
                    else
                        Advance();
                    return actionNode;
                }
                Advance();
            }
            return actionNode;
        }
        Advance();
        if (currTokenVal() != "{")
        {
            ReportError("Token inesperado, se esperaba '{'");
        }
        actionNode.Right = ParseBlock();
        if (currTokenVal() != "}")
        {
            ReportError("Token inesperado, se debe concluir la declaración con un '}'");
        }
        else Advance();
        if (currTokenVal() != "}")
        {
            if (currTokenVal() != ",")
            {
                ReportError("Token inesperado en la declaración de effect");
                return actionNode;
            }
            else
            {
                if (Peek().Value is string && (string)Peek().Value == "}")
                {
                    ReportError("La coma es innecesaria");
                }
                Advance();
                return actionNode;
            }
        }
        else return actionNode;
    }

    public ASTNode ParseBlock()
    {
        var blockNode = new MultiChildNode(NodeType.Block, "Block", currentToken.Line, currentToken.Pos);

        while (currTokenVal() != "}")
        {
            var statement = ParseStatement();
            if (statement != null)
            {
                blockNode.AddChild(statement);
            }
            else
            {
                break;
            }
        }

        return blockNode;
    }

    public ASTNode? ParseStatement()
    {
        if (currTokenVal() == "if")
        {
            return ParseIfStatement();
        }
        else if (currTokenVal() == "while")
        {
            return ParseWhileStatement();
        }
        else if (currTokenVal() == "for")
        {
            return ParseForStatement();
        }
        else if (currentToken.TokenType == TokenType.Identifier && Peek().Value is string && (string)Peek().Value == ".")
        {
            return ParseFunctStatement();
        }
        else if (currentToken.TokenType == TokenType.Identifier)
        {
            return ParseAssignment();
        }
        else
        {
            ReportError("Token inesperado, no hay ningún tipo de declaración que comience con ese token");
            return null;
        }
    }

    public ASTNode ParseIfStatement()
    {
        int line = currentToken.Line;
        int pos = currentToken.Pos;
        Advance();

        if (currTokenVal() != "(")
        {
            ReportError("Se esperaba '(' después de 'if'");
        }
        Advance();

        var condition = ParseExpression();

        if (currTokenVal() != ")")
        {
            ReportError("Se esperaba ')' después de la condición 'if'");
        }
        Advance();

        if (currTokenVal() != "{")
        {
            ReportError("Se esperaba '{' antes del bloque de código del 'if'");
        }
        Advance();

        var body = ParseBlock();

        if (currTokenVal() != "}")
        {
            ReportError("Se esperaba '}' después del bloque de código del 'if'");
        }
        Advance();

        var ifNode = new BinaryNode(NodeType.Statement, "if", line, pos, condition, body);

        return ifNode;
    }

    public ASTNode ParseWhileStatement()
    {
        int line = currentToken.Line;
        int pos = currentToken.Pos;
        Advance();

        if (currTokenVal() != "(")
        {
            ReportError("Se esperaba '(' después de 'while'");
        }
        Advance();

        var condition = ParseExpression();

        if (currTokenVal() != ")")
        {
            ReportError("Se esperaba ')' después de la condición 'while'");
        }
        Advance();

        if (currTokenVal() != "{")
        {
            ReportError("Se esperaba '{' antes del bloque de código del 'while'");
        }
        Advance();

        var body = ParseBlock();

        if (currTokenVal() != "}")
        {
            ReportError("Se esperaba '}' después del bloque de código del 'while'");
        }
        Advance();

        var whileNode = new BinaryNode(NodeType.Statement, "while", line, pos, condition, body);

        return whileNode;
    }

    public ASTNode ParseForStatement()
    {
        int line = currentToken.Line;
        int pos = currentToken.Pos;
        Advance();

        if (currTokenVal() != "(")
        {
            ReportError("Se esperaba '(' después de 'for'");
        }
        Advance();

        if (currentToken.TokenType != TokenType.Identifier)
        {
            ReportError("Se esperaba un identificador después de 'for('");
        }
        var variable = currentToken;
        Advance();

        if (currTokenVal() != "in")
        {
            ReportError("Se esperaba 'in' después del identificador en 'for'");
        }
        Advance();

        if (currentToken.TokenType != TokenType.Identifier)
        {
            ReportError("Se esperaba un identificador después de 'in'");
        }
        var collection = currentToken;
        Advance();

        if (currTokenVal() != ")")
        {
            ReportError("Se esperaba ')' después de la expresión 'for'");
        }
        Advance();

        if (currTokenVal() != "{")
        {
            ReportError("Se esperaba '{' antes del bloque de código del 'for'");
        }
        Advance();

        var body = ParseBlock();

        if (currTokenVal() != "}")
        {
            ReportError("Se esperaba '}' después del bloque de código del 'for'");
        }
        Advance();

        var forNode = new MultiChildNode(NodeType.Statement, "for", line, pos);
        forNode.AddChild(new ASTNode(NodeType.Identifier, variable.Line, variable.Pos, variable.Value));
        forNode.AddChild(new ASTNode(NodeType.Identifier, collection.Line, collection.Pos, collection.Value));
        forNode.AddChild(body);

        return forNode;
    }

    public ASTNode ParseAssignment()
    {
        int line = currentToken.Line;
        int pos = currentToken.Pos;

        if (currentToken.TokenType != TokenType.Identifier)
        {
            ReportError("Se esperaba un identificador en el lado izquierdo de la asignación");
        }
        var left = new ASTNode(NodeType.Identifier, currentToken.Line, currentToken.Pos, currentToken.Value);
        Advance();

        if (currTokenVal() != "=")
        {
            ReportError("Se esperaba '=' en la asignación");
        }
        Advance();

        var right = ParseExpression();

        var assignmentNode = new BinaryNode(NodeType.Assigment, "=", line, pos, left, right);
        return assignmentNode;
    }
    private ASTNode ParseFunctStatement()
    {
        var functionCall = new BinaryNode(NodeType.FunctionCall, "", currentToken.Line, currentToken.Pos, new ASTNode(NodeType.Identifier, currentToken.Line, currentToken.Pos, currentToken.Value));
        Advance();
        while (currTokenVal() == ".")
        {
            Advance();
            if (currentToken.TokenType != TokenType.Identifier)
            {
                ReportError("Token inesperado, se esperaba un identificador");
                return functionCall;
            }
            if (Peek().Value is string && (string)Peek().Value == "(")
            {
                var function = new BinaryNode(NodeType.Function, "", currentToken.Line, currentToken.Pos, new ASTNode(NodeType.Identifier, currentToken.Line, currentToken.Pos, currentToken.Value));
                functionCall = new BinaryNode(NodeType.FunctionCall, "", currentToken.Line, currentToken.Pos, functionCall, function);
                Advance();
                Advance();
                if (currentToken.TokenType == TokenType.Identifier)
                {
                    function.Right = new ASTNode(NodeType.Identifier, currentToken.Line, currentToken.Pos, currentToken.Value);
                    Advance();
                }
                if (currTokenVal() != ")")
                {
                    ReportError("Token inesperado, se esperaba ')'");
                    return functionCall;
                }
                Advance();
                return functionCall;
            }
            functionCall = new BinaryNode(NodeType.PropertyAcces, "", currentToken.Line, currentToken.Pos, functionCall, new ASTNode(NodeType.Identifier, currentToken.Line, currentToken.Pos, currentToken.Value));
            Advance();
        }
        ReportError("Token inesperado, se esperaba un '.' o '('");
        return functionCall;
    }

    private ASTNode? ParseExpression()
    {
        var left = ParseLogicalOr();
        return left;
    }

    private ASTNode? ParseLogicalOr()
    {
        var left = ParseLogicalAnd();
        if (left == null) return null;

        while (currTokenVal() == "||")
        {
            var operatorToken = currentToken;
            Advance();
            var right = ParseLogicalAnd();
            if (right == null)
            {
                ReportError($"Se esperaba una expresión después de '||'");
                return null;
            }

            left = new BinaryNode(NodeType.BinaryOp, operatorToken.Value, operatorToken.Line, operatorToken.Pos, left, right);
        }

        return left;
    }

    private ASTNode? ParseLogicalAnd()
    {
        var left = ParseEquality();
        if (left == null) return null;

        while (currTokenVal() == "&&")
        {
            var operatorToken = currentToken;
            Advance();
            var right = ParseEquality();
            if (right == null)
            {
                ReportError($"Se esperaba una expresión después de '&&'");
                return null;
            }

            left = new BinaryNode(NodeType.BinaryOp, operatorToken.Value, operatorToken.Line, operatorToken.Pos, left, right);
        }

        return left;
    }

    private ASTNode? ParseEquality()
    {
        var left = ParseComparison();
        if (left == null) return null;

        while (currTokenVal() == "==" || currTokenVal() == "!=")
        {
            var operatorToken = currentToken;
            Advance();
            var right = ParseComparison();
            if (right == null)
            {
                ReportError($"Se esperaba una expresión después de '{operatorToken.Value}'");
                return null;
            }

            left = new BinaryNode(NodeType.BinaryOp, operatorToken.Value, operatorToken.Line, operatorToken.Pos, left, right);
        }

        return left;
    }

    private ASTNode? ParseComparison()
    {
        var left = ParseConcatenation();
        if (left == null) return null;

        while (currTokenVal() == "<" || currTokenVal() == ">" ||
               currTokenVal() == "<=" || currTokenVal() == ">=")
        {
            var operatorToken = currentToken;
            Advance();
            var right = ParseConcatenation();
            if (right == null)
            {
                ReportError($"Se esperaba una expresión después de '{operatorToken.Value}'");
                return null;
            }

            left = new BinaryNode(NodeType.BinaryOp, operatorToken.Value, operatorToken.Line, operatorToken.Pos, left, right);
        }

        return left;
    }

    private ASTNode? ParseConcatenation()
    {
        var left = ParseTerm();
        if (left == null) return null;

        while (currTokenVal() == "@" || currTokenVal() == "@@")
        {
            var operatorToken = currentToken;
            Advance();
            var right = ParseTerm();
            if (right == null)
            {
                ReportError($"Se esperaba una expresión después de '{operatorToken.Value}'");
                return null;
            }

            left = new BinaryNode(NodeType.BinaryOp, operatorToken.Value, operatorToken.Line, operatorToken.Pos, left, right);
        }

        return left;
    }

    private ASTNode? ParseTerm()
    {
        var left = ParseFactor();
        if (left == null) return null;

        while (currTokenVal() == "+" || currTokenVal() == "-")
        {
            var operatorToken = currentToken;
            Advance();
            var right = ParseFactor();
            if (right == null)
            {
                ReportError($"Se esperaba una expresión después de '{operatorToken.Value}'");
                return null;
            }

            left = new BinaryNode(NodeType.BinaryOp, operatorToken.Value, operatorToken.Line, operatorToken.Pos, left, right);
        }

        return left;
    }

    private ASTNode? ParseFactor()
    {
        var left = ParseUnary();
        if (left == null) return null;

        while (currTokenVal() == "*" || currTokenVal() == "/")
        {
            var operatorToken = currentToken;
            Advance();
            var right = ParseUnary();
            if (right == null)
            {
                ReportError($"Se esperaba una expresión después de '{operatorToken.Value}'");
                return null;
            }

            left = new BinaryNode(NodeType.BinaryOp, operatorToken.Value, operatorToken.Line, operatorToken.Pos, left, right);
        }

        return left;
    }

    private ASTNode? ParseUnary()
    {
        if (currTokenVal() == "-" || currTokenVal() == "++" || currTokenVal() == "--")
        {
            var operatorToken = currentToken;
            Advance();
            var right = ParsePrimary();
            if (right == null)
            {
                ReportError($"Se esperaba una expresión después de '{operatorToken.Value}'");
                return null;
            }

            return new UnaryNode(NodeType.UnaryOp, operatorToken.Value, operatorToken.Line, operatorToken.Pos, right);
        }

        return ParsePrimary();
    }

    private ASTNode? ParsePrimary()
    {

        if (currentToken.TokenType == TokenType.Number || currentToken.TokenType == TokenType.String)
        {
            Advance();
            return new ASTNode(NodeType.Literal, currentToken.Line, currentToken.Pos, currentToken.Value);
        }
        else if (currentToken.TokenType == TokenType.Identifier)
        {
            var identifierNode = new ASTNode(NodeType.Identifier, currentToken.Line, currentToken.Pos, currentToken.Value);
            Advance();
            while (currTokenVal() == ".")
            {
                Advance();
                if (currentToken.TokenType != TokenType.Identifier)
                {
                    ReportError("Token inesperado, se esperaba un identificador");
                    return identifierNode;
                }
                if (Peek().Value is string && (string)Peek().Value != "(")
                {
                    identifierNode = new BinaryNode(NodeType.PropertyAcces, ".", currentToken.Line, currentToken.Pos, identifierNode, new ASTNode(NodeType.Identifier, currentToken.Line, currentToken.Pos, currentToken.Value));
                    Advance();
                }
                else
                {
                    if (currTokenVal() == "Find")
                    {
                        var function = new BinaryNode(NodeType.Function, "", currentToken.Line, currentToken.Pos, new ASTNode(NodeType.Identifier, currentToken.Line, currentToken.Pos, currentToken.Value));
                        identifierNode = new BinaryNode(NodeType.FunctionCall, "", currentToken.Line, currentToken.Pos, identifierNode, function);
                        Advance();
                        if (currTokenVal() != "(")
                        {
                            ReportError("Token inesperado, se esperaba '('");
                            return identifierNode;
                        }
                        Advance();
                        function.Right = ParsePredicate();
                        if (currTokenVal() != ")")
                        {
                            ReportError("Token inesperado, se esperaba ')'");
                            return identifierNode;
                        }
                        Advance();
                        return identifierNode;
                    }
                    var funtionName = new ASTNode(NodeType.Identifier, currentToken.Line, currentToken.Pos, currentToken.Value);
                    Advance();
                    Advance();
                    if (currentToken.TokenType == TokenType.Identifier)
                    {
                        funtionName = new BinaryNode(NodeType.Function, "", currentToken.Line, currentToken.Pos, funtionName, new ASTNode(NodeType.Identifier, currentToken.Line, currentToken.Pos, currentToken.Value));
                        Advance();
                    }
                    identifierNode = new BinaryNode(NodeType.FunctionCall, "", currentToken.Line, currentToken.Pos, identifierNode, funtionName);
                    if (currTokenVal() != ")")
                    {
                        ReportError("Token inesperado, se esperaba ')'");
                        return identifierNode;
                    }
                    Advance();
                    return identifierNode;
                }
            }
            return identifierNode;
        }
        else if (currTokenVal() == "(")
        {
            Advance();
            var expr = ParseExpression();
            if (currTokenVal() != ")")
            {
                ReportError("Se esperaba ')'");
                return null;
            }
            Advance();
            return expr;
        }
        else
        {
            ReportError($"Token inesperado: {currentToken.Value}");
            return null;
        }
    }

    private ASTNode? ParsePredicate()
    {
        if (currTokenVal() != "(")
        {
            ReportError("Token inesperado, se esperaba '('");
            return null;
        }
        Advance();
        if (currentToken.TokenType != TokenType.Identifier)
        {
            ReportError("Token inesperado, se esperaba un identificador");
            return null;
        }
        var predicate = new BinaryNode(NodeType.Predicate, "", currentToken.Line, currentToken.Pos, new ASTNode(NodeType.Identifier, currentToken.Line, currentToken.Pos, currentToken.Value));
        Advance();
        if (currTokenVal() != ")")
        {
            ReportError("Token inesperado, se esperaba ')'");
            return null;
        }
        Advance();
        if (currTokenVal() != "=>")
        {
            ReportError("Token inesperado, se esperaba '=>'");
            return null;
        }
        Advance();
        predicate.Right = ParseExpression();
        return predicate;
    }

    private ASTNode ParseCardDecl()
    {
        var cardNode = new MultiChildNode(NodeType.CardDecl, "card", currentToken.Line, currentToken.Pos);
        Advance();
        if (currTokenVal() != "{")
        {
            ReportError("Token inesperado, se esperaba '{'");
        }
        else Advance();
        while (currTokenVal() != "}")
        {
            if (currTokenVal() == "card" || currTokenVal() == "effect")
            {
                ReportError("Declaración de carta incompleta, se debe terminar con '}'");
                return cardNode;
            }
            if (currTokenVal() == "Type")
            {
                var typeNode = ParseCardDeclType();
                cardNode.AddChild(typeNode);
            }
            if (currTokenVal() == "Name")
            {
                var nameNode = ParseCardDeclName();
                cardNode.AddChild(nameNode);
            }
            if (currTokenVal() == "Faction")
            {
                var factionNode = ParseCardDeclFaction();
                cardNode.AddChild(factionNode);
            }
            if (currTokenVal() == "Power")
            {
                var powerNode = ParseCardDeclPower();
                cardNode.AddChild(powerNode);
            }
            if (currTokenVal() == "Range")
            {
                var rangeNode = ParseCardDeclRange();
                cardNode.AddChild(rangeNode);
            }
            if (currTokenVal() == "OnAct")
            {
                var onActNode = ParseCardDeclOnAct();
                cardNode.AddChild(onActNode);
            }
        }
        if (currTokenVal() == "}") Advance();
        return cardNode;
    }
    private ASTNode ParseCardDeclType()
    {
        var typeNode = new UnaryNode(NodeType.Property, currentToken.Value, currentToken.Line, currentToken.Pos);
        Advance();
        if (currTokenVal() != ":")
        {
            ReportError("Token inesperado, se esperaba ':'");
            while (currTokenVal() != "Type" && currTokenVal() != "Name" && currTokenVal() != "Faction" && currTokenVal() != "Power" && currTokenVal() != "Range" && currTokenVal() != "OnActivation" && currTokenVal() != "card" && currTokenVal() != "effect")
            {
                Advance();
            }
            return typeNode;
        }
        Advance();
        if (currentToken.TokenType != TokenType.String)
        {
            ReportError("Token inesperado, se esperaba un token de tipo string");
            while (currTokenVal() != "Type" && currTokenVal() != "Name" && currTokenVal() != "Faction" && currTokenVal() != "Power" && currTokenVal() != "Range" && currTokenVal() != "OnActivation" && currTokenVal() != "card" && currTokenVal() != "effect")
            {
                Advance();
            }
            return typeNode;
        }
        typeNode.Child = new ASTNode(NodeType.Literal, currentToken.Line, currentToken.Pos, currentToken.Value);
        Advance();
        if (currTokenVal() != "}")
        {
            if (currTokenVal() != ",")
            {
                ReportError("Token inesperado en la declaración de effect");
                return typeNode;
            }
            else
            {
                if (Peek().Value is string && (string)Peek().Value == "}")
                {
                    ReportError("La coma es innecesaria");
                }
                Advance();
                return typeNode;
            }
        }
        else return typeNode;
    }
    private ASTNode ParseCardDeclName()
    {
        var nameNode = new UnaryNode(NodeType.Property, currentToken.Value, currentToken.Line, currentToken.Pos);
        Advance();
        if (currTokenVal() != ":")
        {
            ReportError("Token inesperado, se esperaba ':'");
            while (currTokenVal() != "Type" && currTokenVal() != "Name" && currTokenVal() != "Faction" && currTokenVal() != "Power" && currTokenVal() != "Range" && currTokenVal() != "OnActivation" && currTokenVal() != "card" && currTokenVal() != "effect")
            {
                Advance();
            }
            return nameNode;
        }
        Advance();
        if (currentToken.TokenType != TokenType.String)
        {
            ReportError("Token inesperado, se esperaba un token de tipo string");
            while (currTokenVal() != "Type" && currTokenVal() != "Name" && currTokenVal() != "Faction" && currTokenVal() != "Power" && currTokenVal() != "Range" && currTokenVal() != "OnActivation" && currTokenVal() != "card" && currTokenVal() != "effect")
            {
                Advance();
            }
            return nameNode;
        }
        nameNode.Child = new ASTNode(NodeType.Literal, currentToken.Line, currentToken.Pos, currentToken.Value);
        Advance();
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
    }
    private ASTNode ParseCardDeclFaction()
    {
        var factionNode = new UnaryNode(NodeType.Property, currentToken.Value, currentToken.Line, currentToken.Pos);
        Advance();
        if (currTokenVal() != ":")
        {
            ReportError("Token inesperado, se esperaba ':'");
            while (currTokenVal() != "Type" && currTokenVal() != "Name" && currTokenVal() != "Faction" && currTokenVal() != "Power" && currTokenVal() != "Range" && currTokenVal() != "OnActivation" && currTokenVal() != "card" && currTokenVal() != "effect")
            {
                Advance();
            }
            return factionNode;
        }
        Advance();
        if (currentToken.TokenType != TokenType.String)
        {
            ReportError("Token inesperado, se esperaba un token de tipo string");
            while (currTokenVal() != "Type" && currTokenVal() != "Name" && currTokenVal() != "Faction" && currTokenVal() != "Power" && currTokenVal() != "Range" && currTokenVal() != "OnActivation" && currTokenVal() != "card" && currTokenVal() != "effect")
            {
                Advance();
            }
            return factionNode;
        }
        factionNode.Child = new ASTNode(NodeType.Literal, currentToken.Line, currentToken.Pos, currentToken.Value);
        Advance();
        if (currTokenVal() != "}")
        {
            if (currTokenVal() != ",")
            {
                ReportError("Token inesperado en la declaración de effect");
                return factionNode;
            }
            else
            {
                if (Peek().Value is string && (string)Peek().Value == "}")
                {
                    ReportError("La coma es innecesaria");
                }
                Advance();
                return factionNode;
            }
        }
        else return factionNode;
    }
    private ASTNode ParseCardDeclPower()
    {
        var powerNode = new UnaryNode(NodeType.Property, currentToken.Value, currentToken.Line, currentToken.Pos);
        Advance();
        if (currTokenVal() != ":")
        {
            ReportError("Token inesperado, se esperaba ':'");
            while (currTokenVal() != "Type" && currTokenVal() != "Name" && currTokenVal() != "Faction" && currTokenVal() != "Power" && currTokenVal() != "Range" && currTokenVal() != "OnActivation" && currTokenVal() != "card" && currTokenVal() != "effect")
            {
                Advance();
            }
            return powerNode;
        }
        Advance();
        if (currentToken.TokenType != TokenType.Number)
        {
            ReportError("Token inesperado, se esperaba un token de tipo Number");
            while (currTokenVal() != "Type" && currTokenVal() != "Name" && currTokenVal() != "Faction" && currTokenVal() != "Power" && currTokenVal() != "Range" && currTokenVal() != "OnActivation" && currTokenVal() != "card" && currTokenVal() != "effect")
            {
                Advance();
            }
            return powerNode;
        }
        powerNode.Child = new ASTNode(NodeType.Literal, currentToken.Line, currentToken.Pos, currentToken.Value);
        Advance();
        if (currTokenVal() != "}")
        {
            if (currTokenVal() != ",")
            {
                ReportError("Token inesperado en la declaración de effect");
                return powerNode;
            }
            else
            {
                if (Peek().Value is string && (string)Peek().Value == "}")
                {
                    ReportError("La coma es innecesaria");
                }
                Advance();
                return powerNode;
            }
        }
        else return powerNode;
    }
    private ASTNode ParseCardDeclRange()
    {
        return new ASTNode(NodeType.Program, 0, 0, 0);
    }
    private ASTNode ParseCardDeclOnAct()
    {
        return new ASTNode(NodeType.Program, 0, 0, 0);
    }
}