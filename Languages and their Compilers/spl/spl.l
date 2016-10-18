%{
	#ifdef PRINT
	#define TOKEN(k, t) printf(#k ": " #t "\n");
	#define VALUE(k, t) printf(#k ": " #t " %s\n", yytext);
	
	#else
	#define TOKEN(k, t) return(t);
	#define VALUE(k, t) return(t);
	
	#endif
%}

delim	[ \t]
white_space	{delim}+
character	[a-zA-Z]
character_constant	"'"{character}"'"
digit	[0-9]
number	{digit}+
identifier	{character}({character}|{digit})*

%%

	int l = 1;

{white_space}	;
{character_constant}	VALUE(Constant, CHARACTER_CONSTANT)
{number}	VALUE(Constant, NUMBER)

":"	TOKEN(Separator, COLON)
"."	TOKEN(Separator, FULL_STOP)
","	TOKEN(Separator, COMMA)
";"	TOKEN(Separator, SEMICOLON)

"->"	TOKEN(Operator, HYPHEN_GREATER_THAN)

"("	TOKEN(Separator, OPEN_BRACKET)
")"	TOKEN(Separator, CLOSE_BRACKET)

"="	TOKEN(Operator, EQUAL)
"<"	TOKEN(Operator, LESS_THAN)
">"	TOKEN(Operator, GREATER_THAN)
"<="	TOKEN(Operator, LESS_THAN_EQUAL)
">="	TOKEN(Operator, GREATER_THAN_EQUAL)
"<>"	TOKEN(Operator, LESS_THAN_GREATER_THAN)

"+"	TOKEN(Operator, PLUS)
"-"	TOKEN(Operator, HYPHEN)
"*"	TOKEN(Operator, ASTERIX)
"/"	TOKEN(Operator, FORWARD_SLASH)

ENDP	TOKEN(Keyword, ENDP)
CODE	TOKEN(Keyword, CODE)
DECLARATIONS	TOKEN(Keyword, DECLARATIONS)

OF	TOKEN(Keyword, OF)
TYPE	TOKEN(Keyword, TYPE)
REAL	TOKEN(Keyword, REAL)
INTEGER	TOKEN(Keyword, INTEGER)
CHARACTER	TOKEN(Keyword, CHARACTER)

IF	TOKEN(Keyword, IF)
THEN	TOKEN(Keyword, THEN)
ENDIF	TOKEN(Keyword, ENDIF)
ELSE	TOKEN(Keyword, ELSE)

DO	TOKEN(Keyword, DO)
WHILE	TOKEN(Keyword, WHILE)
ENDDO	TOKEN(Keyword, ENDDO)
ENDWHILE	TOKEN(Keyword, ENDWHILE)

FOR	TOKEN(Keyword, FOR)
IS	TOKEN(Keyword, IS)
BY	TOKEN(Keyword, BY)
TO	TOKEN(Keyword, TO)
ENDFOR	TOKEN(Keyword, ENDFOR)

WRITE	TOKEN(Keyword, WRITE)
NEWLINE	TOKEN(Keyword, NEWLINE)
READ	TOKEN(Keyword, READ)

NOT	TOKEN(Keyword, NOT)
OR	TOKEN(Keyword, OR)
AND	TOKEN(Keyword, AND)

{identifier}	VALUE(Identifier, IDENTIFIER)

"\n"	l++;

. printf("ERROR unrecognised character on line %d: %s\n", yytext, l);

%%