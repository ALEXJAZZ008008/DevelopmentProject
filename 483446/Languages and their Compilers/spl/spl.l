%{
	#include <string.h>
	
	#ifdef PRINT
		#define TOKEN(k, t) printf(#k ": " #t "\n");
		#define VALUE_C(k, t) printf(#k ": " #t " %c\n", yytext[1]);
		#define VALUE_N(k, t) printf(#k ": " #t " %s\n", yytext);
		#define VALUE_I(k, t) printf(#k ": " #t " %s\n", yytext);
	#else
		#define TOKEN(k, t) return(t);
		#define VALUE_C(k, t) yylval.iVal = yytext[1]; return(t);
		#define VALUE_N(k, t) yylval.iVal = atoi(yytext); return(t);
		#define VALUE_I(k, t) yylval.iVal = installId(yytext); return(t);
		
		extern symbolTableNodePointer symbolTable[SYMTABSIZE];
		extern int currentSymbolTableSize;
		
		int installId(char *);
	#endif
%}

delimiter	[ \r\t]
white_space	{delimiter}+
character	[a-zA-Z]
character_constant	"'"{character}"'"
digit	[0-9]
number	{digit}+
identifier	{character}({character}|{digit})*

%%

	int l = 1;

{white_space}	;
{character_constant}	VALUE_C(Constant, CHARACTER_CONSTANT)
{number}	VALUE_N(Constant, NUMBER)

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
TYPE	TOKEN(Keyword, TYPE_L)
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

{identifier}	VALUE_I(Identifier, IDENTIFIER)

"\n"	l++;

. printf("ERROR unrecognised character on line %d: %s (%d)\n", l, yytext, yytext[0]);

%%

#ifndef PRINT
	symbolTableNodePointer newsymbolTableNode()
	{
		return((symbolTableNodePointer)malloc(sizeof(symbolTableNode)));
	}

	int lookup(char * s)
	{
		extern symbolTableNodePointer symbolTable[SYMTABSIZE];
		extern int currentSymbolTableSize;
		
		for(int i = 0; i < currentSymbolTableSize; i++)
		{
			if(strncmp(s, symbolTable[i] -> identifier, IDLENGTH) == 0)
			{
				return (i);
			}
		}
		
		return (-1);    
	}

	int installId(char * id)
	{
		extern symbolTableNodePointer symbolTable[SYMTABSIZE];
		extern int currentSymbolTableSize;
		int index = lookup(id);
		
		if(index >= 0)
		{
			return (index);
		}
		else
		{
			if(currentSymbolTableSize >= SYMTABSIZE) 
			{
				return (NOTHING) ;
			}
			else
			{
				symbolTable[currentSymbolTableSize] = newsymbolTableNode();
				strncpy(symbolTable[currentSymbolTableSize] -> identifier, id, IDLENGTH);
				symbolTable[currentSymbolTableSize] -> identifier[IDLENGTH-1] = '\0';
				
				return(currentSymbolTableSize++);
			}
		}
	}
#endif