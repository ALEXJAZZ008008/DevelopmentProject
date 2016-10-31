%{	
	#include <stdio.h>
	#include <stdlib.h>
	
	#define SYMTABSIZE 50
	#define IDLENGTH 15
	#define NOTHING -1
	#define INDENTOFFSET 2
	
	#ifndef TRUE
	#define TRUE 1
	#endif
	
	#ifndef FALSE
	#define FALSE 0
	#endif

	#ifndef NULL
	define NULL 0
	#endif
	
	int yylex(void);
	void yyerror(char * s);
	
	enum ParseTreeNodeType {PROGRAM, BLOCK, DECLARATION_LIST, DECLARATION, TYPE_Y, STATEMENT_LIST, STATEMENT, ASSIGNMENT_STATEMENT, IF_STATEMENT, DO_STATEMENT, WHILE_STATEMENT, FOR_STATEMENT, WRITE_STATEMENT,
							READ_STATEMENT, OUTPUT_LIST, CONDITIONAL_LIST, CONDITIONAL, COMPARATOR, EXPRESSION, TERM, VALUE, CONSTANT, NUMBER_CONSTANT, IDENTIFIER_LIST};
	
	struct treeNode
	{
		int item;
		int nodeIdentifier;
		struct treeNode * first;
		struct treeNode * second;
		struct treeNode * third;
	};
	
	struct symTabNode
	{
		char identifier[IDLENGTH];
	};
	
	typedef struct treeNode TREE_NODE;
	typedef TREE_NODE * TERNARY_TREE;
	
	typedef struct symTabNode SYMTABNODE;
	typedef SYMTABNODE * SYMTABNODEPTR;
	
	int currentSymTabSize = 0;
	
	SYMTABNODEPTR symTab[SYMTABSIZE]; 
	TERNARY_TREE create_node(int, int, TERNARY_TREE, TERNARY_TREE, TERNARY_TREE);
	void printTree(TERNARY_TREE);
	void generateCode(TERNARY_TREE);
%}

%start  program

%union
{
    int iVal;
    TERNARY_TREE tVal;
}

%token<iVal> IDENTIFIER CHARACTER_CONSTANT NUMBER

%token	COLON FULL_STOP COMMA SEMICOLON HYPHEN_GREATER_THAN OPEN_BRACKET CLOSE_BRACKET EQUAL LESS_THAN GREATER_THAN LESS_THAN_EQUAL GREATER_THAN_EQUAL LESS_THAN_GREATER_THAN PLUS HYPHEN ASTERIX
		FORWARD_SLASH ENDP CODE DECLARATIONS OF TYPE_L REAL INTEGER CHARACTER IF THEN ENDIF ELSE DO WHILE ENDDO ENDWHILE FOR IS BY TO ENDFOR WRITE NEWLINE READ NOT OR AND 

%type<tVal>	program block declaration_list declaration type_y statement_list statement assignment_statement if_statement do_statement while_statement for_statement write_statement read_statement output_list conditional_list
			conditional comparator expression term value constant number_constant identifier_list

%%

program :	IDENTIFIER COLON block ENDP IDENTIFIER FULL_STOP
			{
				TERNARY_TREE ParseTree = create_node($1, PROGRAM, $3, NULL, NULL);
				
				#ifdef DEBUG
					printTree(ParseTree);
				#endif
				
				generateCode(ParseTree);
			}
			;

block	:	CODE statement_list
			{
				$$ = create_node(NOTHING, BLOCK, $2, NULL, NULL);
			}
			| DECLARATIONS declaration_list CODE statement_list
			{
				$$ = create_node(NOTHING, BLOCK, $2, $4, NULL);
			}
			;

declaration_list	:	declaration
						{
							$$ = create_node(NOTHING, DECLARATION_LIST, $1, NULL, NULL);
						}
						| declaration declaration_list
						{
							$$ = create_node(NOTHING, DECLARATION_LIST, $1, $2, NULL);
						}
						;

declaration	:	identifier_list OF TYPE_L type_y SEMICOLON
				{
					$$ = create_node(NOTHING, DECLARATION, $1, $4, NULL);
				}
				;

type_y	:	REAL
			{
				$$ = create_node(REAL, TYPE_Y, NULL, NULL, NULL);
			}
			| INTEGER
			{
				$$ = create_node(INTEGER, TYPE_Y, NULL, NULL, NULL);
			}
			| CHARACTER
			{
				$$ = create_node(CHARACTER, TYPE_Y, NULL, NULL, NULL);
			}
			;

statement_list	:	statement
					{
						$$ = create_node(NOTHING, STATEMENT_LIST, $1, NULL, NULL);
					}
					| statement SEMICOLON statement_list
					{
						$$ = create_node(NOTHING, STATEMENT_LIST, $1, $3, NULL);
					}
					;

statement	:	do_statement
				{
					$$ = create_node(DO_STATEMENT, STATEMENT, $1, NULL, NULL);
				}
				| if_statement
				{
					$$ = create_node(IF_STATEMENT, STATEMENT, $1, NULL, NULL);
				}
				| for_statement
				{
					$$ = create_node(FOR_STATEMENT, STATEMENT, $1, NULL, NULL);
				}
				| read_statement
				{
					$$ = create_node(READ_STATEMENT, STATEMENT, $1, NULL, NULL);
				}
				| while_statement
				{
					$$ = create_node(WHILE_STATEMENT, STATEMENT, $1, NULL, NULL);
				}
				| write_statement
				{
					$$ = create_node(WRITE_STATEMENT, STATEMENT, $1, NULL, NULL);
				}
				| assignment_statement
				{
					$$ = create_node(ASSIGNMENT_STATEMENT, STATEMENT, $1, NULL, NULL);
				}
				;

assignment_statement	:	expression HYPHEN_GREATER_THAN IDENTIFIER
							{
								$$ = create_node($3, ASSIGNMENT_STATEMENT, $1, NULL, NULL);
							}
							;

if_statement	:	IF conditional_list THEN statement_list ENDIF
					{
						$$ = create_node(NOTHING, IF_STATEMENT, $2, $4, NULL);
					}
					| IF conditional_list THEN statement_list ELSE statement_list ENDIF
					{
						$$ = create_node(NOTHING, IF_STATEMENT, $2, $4, $6);
					}
					;

do_statement	:	DO statement_list WHILE conditional_list ENDDO
					{
						$$ = create_node(NOTHING, DO_STATEMENT, $2, $4, NULL);
					}
					;

while_statement	:	WHILE conditional_list DO statement_list ENDWHILE
					{
						$$ = create_node(NOTHING, WHILE_STATEMENT, $2, $4, NULL);
					}
					;

for_statement	:	FOR IDENTIFIER IS expression BY expression TO expression DO statement_list ENDFOR
					{
						$$ = create_node($2, FOR_STATEMENT, create_node(NOTHING, FOR_STATEMENT, $4, $6, $8), $10, NULL);
					}
					;

write_statement	:	NEWLINE
					{
						$$ = create_node(NOTHING, WRITE_STATEMENT, NULL, NULL, NULL);
					}
					| WRITE OPEN_BRACKET output_list CLOSE_BRACKET
					{
						$$ = create_node(NOTHING, WRITE_STATEMENT, $3, NULL, NULL);
					}
					;

read_statement	:	READ OPEN_BRACKET IDENTIFIER CLOSE_BRACKET
					{
						$$ = create_node($3, READ_STATEMENT, NULL, NULL, NULL);
					}
					;

output_list	:	value
				{
					$$ = create_node(NOTHING, OUTPUT_LIST, $1, NULL, NULL);
				}
				| value COMMA output_list
				{
					$$ = create_node(NOTHING, OUTPUT_LIST, $1, $3, NULL);
				}
				;

conditional_list	:	conditional
						{
							$$ = create_node(NOTHING, CONDITIONAL_LIST, $1, NULL, NULL);
						}
						| conditional OR conditional
						{
							$$ = create_node(OR, CONDITIONAL_LIST, $1, $3, NULL);
						}
						| conditional AND conditional
						{
							$$ = create_node(AND, CONDITIONAL_LIST, $1, $3, NULL);
						}
						;

conditional	:	NOT conditional
				{
					$$ = create_node(NOT, CONDITIONAL, $2, NULL, NULL);
				}
				| expression comparator expression
				{
					$$ = create_node(NOTHING, CONDITIONAL, $1, $2, $3);
				}

comparator	:	EQUAL
				{
					$$ = create_node(EQUAL, COMPARATOR, NULL, NULL, NULL);
				}
				| LESS_THAN
				{
					$$ = create_node(LESS_THAN, COMPARATOR, NULL, NULL, NULL);
				}
				| GREATER_THAN
				{
					$$ = create_node(GREATER_THAN, COMPARATOR, NULL, NULL, NULL);
				}
				| LESS_THAN_EQUAL
				{
					$$ = create_node(LESS_THAN_EQUAL, COMPARATOR, NULL, NULL, NULL);
				}
				| GREATER_THAN_EQUAL
				{
					$$ = create_node(GREATER_THAN_EQUAL, COMPARATOR, NULL, NULL, NULL);
				}
				| LESS_THAN_GREATER_THAN
				{
					$$ = create_node(LESS_THAN_GREATER_THAN, COMPARATOR, NULL, NULL, NULL);
				}
				;

expression	:	term
				{
					$$ = create_node(NOTHING, EXPRESSION, $1, NULL, NULL);
				}
				| term PLUS expression
				{
					$$ = create_node(PLUS, EXPRESSION, $1, $3, NULL);
				}
				| term HYPHEN expression
				{
					$$ = create_node(HYPHEN, EXPRESSION, $1, $3, NULL);
				}
				;

term	:	value
			{
				$$ = create_node(NOTHING, TERM, $1, NULL, NULL);
			}
			| value ASTERIX term
			{
				$$ = create_node(ASTERIX, TERM, $1, $3, NULL);
			}
			| value FORWARD_SLASH term
			{
				$$ = create_node(FORWARD_SLASH, TERM, $1, $3, NULL);
			}
			;

value	:	constant
			{
				$$ = create_node(NOTHING, VALUE, $1, NULL, NULL);
			}
			| IDENTIFIER
			{
				$$ = create_node($1, VALUE, NULL, NULL, NULL);
			}
			| OPEN_BRACKET expression CLOSE_BRACKET
			{
				$$ = create_node(NOTHING, VALUE, $2, NULL, NULL);
			}
			;

constant	:	number_constant
				{
					$$ = create_node(NOTHING, CONSTANT, $1, NULL, NULL);
				}
				| CHARACTER_CONSTANT
				{
					$$ = create_node($1, CONSTANT, NULL, NULL, NULL);
				}
				;

number_constant	:	NUMBER
					{
						$$ = create_node($1, NUMBER_CONSTANT, NULL, NULL, NULL);
					}
					| HYPHEN NUMBER
					{
						$$ = create_node($2, NUMBER_CONSTANT, NULL, NULL, NULL);
					}
					| NUMBER FULL_STOP NUMBER
					{
						$$ = create_node($1, NUMBER_CONSTANT, create_node($3, NUMBER_CONSTANT, NULL, NULL, NULL), NULL, NULL);
					}
					| HYPHEN NUMBER FULL_STOP NUMBER
					{
						$$ = create_node($2, NUMBER_CONSTANT, create_node($4, NUMBER_CONSTANT, NULL, NULL, NULL), NULL, NULL);
					}
					;

identifier_list	:	IDENTIFIER
					{
						$$ = create_node($1, IDENTIFIER_LIST, NULL, NULL, NULL);
					}
					| identifier_list COMMA IDENTIFIER
					{
						$$ = create_node(IDENTIFIER, IDENTIFIER_LIST, $1, NULL, NULL);
					}
					;

%%

TERNARY_TREE create_node(int ival, int case_identifier, TERNARY_TREE p1, TERNARY_TREE  p2, TERNARY_TREE  p3)
{
    TERNARY_TREE t;
    t = (TERNARY_TREE)malloc(sizeof(TREE_NODE));
    t -> item = ival;
    t -> nodeIdentifier = case_identifier;
    t -> first = p1;
    t -> second = p2;
    t -> third = p3;
    return (t);
}

void printTree(TERNARY_TREE t)
{
	if (t == NULL)
	{
		return;
	}
	
	printf("Item: %d", t -> item);
	printf(" nodeIdentifier: %d\n", t -> nodeIdentifier);
	
	printTree(t -> first);
	printTree(t -> second);
	printTree(t -> third);
}

void generateCode(TERNARY_TREE t)
{
	if (t == NULL)
	{
		return;
	}
	
	switch(t -> nodeIdentifier)
	{
		case PROGRAM:
			printf("#include <stdio.h>\n");
			printf("int main(void)\n{\n");
			generateCode(t -> first);
			printf("}\n");
			
			break;
			
		case WRITE_STATEMENT:
			printf("printf(");
			
			if(t -> first)
			{
				printf("\"");
				generateCode(t -> first);
				printf("\"");
			}
			else
			{
				printf("\"\\n\"");
			}
			
			printf(");\n");
			
			break;
		
		case CONSTANT:
			printf("%c", t -> item);
			
			break;
		
		default:
			generateCode(t -> first);
			generateCode(t -> second);
			generateCode(t -> third);
			
			break;
	}
}

#include "lex.yy.c"