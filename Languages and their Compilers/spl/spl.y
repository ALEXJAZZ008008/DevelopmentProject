%{
	void yyerror(char * s);
	int yylex(void);
%}

%token	CHARACTER_CONSTANT NUMBER COLON FULL_STOP COMMA SEMICOLON HYPHEN_GREATER_THAN OPEN_BRACKET CLOSE_BRACKET EQUAL LESS_THAN GREATER_THAN LESS_THAN_EQUAL GREATER_THAN_EQUAL LESS_THAN_GREATER_THAN PLUS HYPHEN ASTERIX
		FORWARD_SLASH ENDP CODE DECLARATIONS OF TYPE REAL INTEGER CHARACTER IF THEN ENDIF ELSE DO WHILE ENDDO ENDWHILE FOR IS BY TO ENDFOR WRITE NEWLINE READ NOT OR AND IDENTIFIER

%%

program :	IDENTIFIER COLON block ENDP IDENTIFIER FULL_STOP
			;

block	:	CODE statement_list
			| DECLARATIONS declaration_list CODE statement_list
			;

declaration_list	:	declaration
						| declaration declaration_list
						;

declaration	:	identifier_list OF TYPE type SEMICOLON
				;

type	:	REAL
			| INTEGER
			| CHARACTER
			;

statement_list	:	statement
					| statement SEMICOLON statement_list
					;

statement	:	do_statement
				| if_statement
				| for_statement
				| read_statement
				| while_statement
				| write_statement
				| assignment_statement
				;

assignment_statement	:	expression HYPHEN_GREATER_THAN IDENTIFIER
							;

if_statement	:	IF conditional THEN statement_list ENDIF
					| IF conditional THEN statement_list ELSE statement_list ENDIF
					;

do_statement	:	DO statement_list WHILE conditional ENDDO
					;

while_statement	:	WHILE conditional DO statement_list ENDWHILE
					;

for_statement	:	FOR IDENTIFIER IS expression BY expression TO expression DO statement_list ENDFOR
					;

write_statement	:	NEWLINE
					| WRITE OPEN_BRACKET output_list CLOSE_BRACKET
					;

read_statement	:	READ OPEN_BRACKET IDENTIFIER CLOSE_BRACKET
					;

output_list	:	value
				| value COMMA output_list
				;

conditional	:	NOT conditional
				| expression comparator expression
				| expression comparator expression OR conditional
				| expression comparator expression AND conditional
				;

comparator	:	EQUAL
				| LESS_THAN
				| GREATER_THAN
				| LESS_THAN_EQUAL
				| GREATER_THAN_EQUAL
				| LESS_THAN_GREATER_THAN
				;

expression	:	term
				| term PLUS expression
				| term HYPHEN expression
				;

term	:	value
			| value ASTERIX term
			| value FORWARD_SLASH term
			;

value	:	constant
			| IDENTIFIER
			| OPEN_BRACKET expression CLOSE_BRACKET
			;

constant	:	number_constant
				| CHARACTER_CONSTANT
				;

number_constant	:	NUMBER
					| HYPHEN NUMBER
					| NUMBER FULL_STOP NUMBER
					| HYPHEN NUMBER FULL_STOP NUMBER
					;

identifier_list	:	IDENTIFIER
					| identifier_list COMMA IDENTIFIER
					;

%%

#include "lex.yy.c"