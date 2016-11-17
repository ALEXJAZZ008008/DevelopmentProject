%{	
	#include <stdio.h>
	#include <stdlib.h>
	#include <string.h>
	
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
		#define NULL 0
	#endif
	
	int yylex(void);
	void yyerror(char * s);
	
	enum parseTreeNodeType	{PROGRAM, BLOCK, DECLARATION_LIST, DECLARATION, TYPE_Y, STATEMENT_LIST, STATEMENT, ASSIGNMENT_STATEMENT, IF_STATEMENT, DO_STATEMENT, WHILE_STATEMENT, FOR_STATEMENT, WRITE_STATEMENT,
							READ_STATEMENT, OUTPUT_LIST, CONDITIONAL_LIST, CONDITIONAL, COMPARATOR, EXPRESSION, TERM, VALUE, CONSTANT, NUMBER_CONSTANT, NEGATIVE_NUMBER_CONSTANT, FLOATING_NUMBER_CONSTANT,
							FLOATING_NEGATIVE_NUMBER_CONSTANT, IDENTIFIER_LIST};
	
	char * nodeName[]	=	{"PROGRAM", "BLOCK", "DECLARATION_LIST", "DECLARATION", "TYPE_Y", "STATEMENT_LIST", "STATEMENT", "ASSIGNMENT_STATEMENT", "IF_STATEMENT", "DO_STATEMENT", "WHILE_STATEMENT", "FOR_STATEMENT",
							"WRITE_STATEMENT", "READ_STATEMENT", "OUTPUT_LIST", "CONDITIONAL_LIST", "CONDITIONAL", "COMPARATOR", "EXPRESSION", "TERM", "VALUE", "CONSTANT", "NUMBER_CONSTANT", "NEGATIVE_NUMBER_CONSTANT",
							"FLOATING_NUMBER_CONSTANT", "FLOATING_NEGATIVE_NUMBER_CONSTANT", "IDENTIFIER_LIST"};
	
	struct treeNode
	{
		int item;
		int nodeIdentifier;
		struct treeNode * first;
		struct treeNode * second;
		struct treeNode * third;
	};
	
	struct symbolTableNode
	{
		char identifier[IDLENGTH];
		char type;
	};
	
	typedef struct treeNode treeNode;
	typedef treeNode * ternaryTree;
	ternaryTree create_node(int, int, ternaryTree, ternaryTree, ternaryTree);
	
	int currentSymbolTableSize = 0;
	char constantUnderscore[IDLENGTH] = "_";
	char underscore[IDLENGTH];
	char identifierType = 'N';
	char expressionCharacterType = 'N';
	typedef struct symbolTableNode symbolTableNode;
	typedef symbolTableNode * symbolTableNodePointer;
	symbolTableNodePointer symbolTable[SYMTABSIZE];
	
	#ifdef DEBUG
		void printIdentifier(ternaryTree);
		void printTree(ternaryTree, int);
	#else
		void expressionType(ternaryTree);
		void printCode(ternaryTree);
	#endif
%}

%start  program

%union
{
    int iVal;
    ternaryTree tVal;
}

%token<iVal> CHARACTER_CONSTANT NUMBER IDENTIFIER

%token	COLON FULL_STOP COMMA SEMICOLON HYPHEN_GREATER_THAN OPEN_BRACKET CLOSE_BRACKET EQUAL LESS_THAN GREATER_THAN LESS_THAN_EQUAL GREATER_THAN_EQUAL LESS_THAN_GREATER_THAN PLUS HYPHEN ASTERIX
		FORWARD_SLASH ENDP CODE DECLARATIONS OF TYPE_L REAL INTEGER CHARACTER IF THEN ENDIF ELSE DO WHILE ENDDO ENDWHILE FOR IS BY TO ENDFOR WRITE NEWLINE READ NOT OR AND 

%type<tVal>	program block declaration_list declaration type_y statement_list statement assignment_statement if_statement do_statement while_statement for_statement write_statement read_statement output_list conditional_list
			conditional comparator expression term value constant number_constant identifier_list

%%

program :	IDENTIFIER COLON block ENDP IDENTIFIER FULL_STOP
			{
				ternaryTree parseTree = create_node($1, PROGRAM, create_node($5, PROGRAM, NULL, NULL, NULL), $3, NULL);
				
				#ifdef DEBUG
					printTree(parseTree, 0);
					
					printf("\n");
				#else
					printCode(parseTree);
				#endif
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

statement	:	assignment_statement
				{
					$$ = create_node(ASSIGNMENT_STATEMENT, STATEMENT, $1, NULL, NULL);
				}
				| if_statement
				{
					$$ = create_node(IF_STATEMENT, STATEMENT, $1, NULL, NULL);
				}
				| do_statement
				{
					$$ = create_node(DO_STATEMENT, STATEMENT, $1, NULL, NULL);
				}
				| while_statement
				{
					$$ = create_node(WHILE_STATEMENT, STATEMENT, $1, NULL, NULL);
				}
				| for_statement
				{
					$$ = create_node(FOR_STATEMENT, STATEMENT, $1, NULL, NULL);
				}
				| write_statement
				{
					$$ = create_node(WRITE_STATEMENT, STATEMENT, $1, NULL, NULL);
				}
				| read_statement
				{
					$$ = create_node(READ_STATEMENT, STATEMENT, $1, NULL, NULL);
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
					$$ = create_node(NOTHING, CONDITIONAL, $2, NULL, NULL);
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
				$$ = create_node(CONSTANT, VALUE, $1, NULL, NULL);
			}
			| IDENTIFIER
			{
				$$ = create_node($1, VALUE, NULL, NULL, NULL);
			}
			| OPEN_BRACKET expression CLOSE_BRACKET
			{
				$$ = create_node(EXPRESSION, VALUE, $2, NULL, NULL);
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
						$$ = create_node($2, NEGATIVE_NUMBER_CONSTANT, NULL, NULL, NULL);
					}
					| NUMBER FULL_STOP NUMBER
					{
						$$ = create_node($1, FLOATING_NUMBER_CONSTANT, create_node($3, NUMBER_CONSTANT, NULL, NULL, NULL), NULL, NULL);
					}
					| HYPHEN NUMBER FULL_STOP NUMBER
					{
						$$ = create_node($2, FLOATING_NEGATIVE_NUMBER_CONSTANT, create_node($4, NUMBER_CONSTANT, NULL, NULL, NULL), NULL, NULL);
					}
					;

identifier_list	:	IDENTIFIER
					{
						$$ = create_node($1, IDENTIFIER_LIST, NULL, NULL, NULL);
					}
					| identifier_list COMMA IDENTIFIER
					{
						$$ = create_node($3, IDENTIFIER_LIST, $1, NULL, NULL);
					}
					;

%%

ternaryTree create_node(int ival, int case_identifier, ternaryTree p1, ternaryTree  p2, ternaryTree  p3)
{
    ternaryTree t;
	
    t = (ternaryTree)malloc(sizeof(treeNode));
	
    t -> item = ival;
    t -> nodeIdentifier = case_identifier;
    t -> first = p1;
    t -> second = p2;
    t -> third = p3;
	
    return(t);
}

#ifdef DEBUG
	void printIdentifier(ternaryTree t)
	{
		if(t -> item < 0 || t -> item > currentSymbolTableSize)
		{
			printf(" Unknown Identifier: %d\n", t -> item);
			return;
		}
		else
		{
			printf(" Identifier: %s\n", symbolTable[t -> item] -> identifier);
		}
	}
	
	void printTree(ternaryTree t, int i)
	{
		if(t != NULL)
		{
			int j = 0;
			int k = 0;
			
			while(j < i)
			{
				while(k < INDENTOFFSET)
				{
					printf(" ");
					
					k++;
				}
				
				j++;
				k = 0;
			}
			
			if(t -> nodeIdentifier < 0 || t -> nodeIdentifier > sizeof(nodeName))
			{
				printf("Unknown nodeIdentifier: %d", t -> nodeIdentifier);
			}
			else
			{
				printf("%s", nodeName[t -> nodeIdentifier]);
			}
			
			if(t -> item != NOTHING)
			{
				switch(t -> nodeIdentifier)
				{
					case PROGRAM:
						printIdentifier(t);
						
						break;
					
					case TYPE_Y:
						printf("\n");
						
						break;
					
					case STATEMENT:
						printf("\n");
						
						break;
					
					case ASSIGNMENT_STATEMENT:
						printIdentifier(t);
						
						break;
					
					case FOR_STATEMENT:
						printIdentifier(t);
						
						break;
					
					case READ_STATEMENT:
						printIdentifier(t);
						
						break;
					
					case CONDITIONAL_LIST:
						printf("\n");
						
						break;
					
					case COMPARATOR:
						printf("\n");
						
						break;
					
					case EXPRESSION:
						printf("\n");
						
						break;
					
					case TERM:
						printf("\n");
						
						break;
					
					case VALUE:
						if(t -> item != CONSTANT || t -> item != EXPRESSION)
						{
							printIdentifier(t);
						}
						
						break;
					
					case CONSTANT:
						printf(" Character: %c\n", t -> item);
						
						break;
						
					case NUMBER_CONSTANT:
						printf(" Number: %d\n", t -> item);
						
						break;
					
					case NEGATIVE_NUMBER_CONSTANT:
						printf(" Number: %d\n", t -> item);
						
						break;
					
					case FLOATING_NUMBER_CONSTANT:
						printf(" Number: %d\n", t -> item);
						
						break;
					
					case FLOATING_NEGATIVE_NUMBER_CONSTANT:
						printf(" Number: %d\n", t -> item);
						
						break;
					
					case IDENTIFIER_LIST:
						printIdentifier(t);
						
						break;
					
					default:
						printf(" %d\n", t -> item);
						
						break;
				}
			}
			else
			{
				printf("\n");
			}
			
			i++;
			
			printTree(t -> first, i);
			printTree(t -> second, i);
			printTree(t -> third, i);
		}
	}
#else
	void expressionType(ternaryTree t)
	{
		if(t != NULL)
		{
			switch(t -> nodeIdentifier)
			{
				case CONSTANT:
					if(expressionCharacterType != 'f' && expressionCharacterType != 'd')
					{
						expressionCharacterType = 'c';
					}
					
					break;
					
				case NUMBER_CONSTANT:
					if(expressionCharacterType != 'f')
					{
						expressionCharacterType = 'd';
					}
					
					break;
				
				case NEGATIVE_NUMBER_CONSTANT:
					if(expressionCharacterType != 'f')
					{
						expressionCharacterType = 'd';
					}
					
					break;
				
				case FLOATING_NUMBER_CONSTANT:
					expressionCharacterType = 'f';
					
					break;
				
				case FLOATING_NEGATIVE_NUMBER_CONSTANT:
					expressionCharacterType = 'f';
					
					break;
				
				case IDENTIFIER_LIST:
					if(t -> first -> item < 0 || t -> first -> item > currentSymbolTableSize)
							{
								printf("Unknown Type: %d\n", t -> item);
								return;
							}
							else
							{
								if(symbolTable[t -> first -> item] -> type == 'N')
								{
									printf("Unknown Type: %d\n", t -> first -> item);
									return;
								}
								else
								{
									switch(symbolTable[t -> first -> item] -> type)
									{
										case 'f':
											expressionCharacterType = 'f';
											
											break;
										
										case 'd':
											if(expressionCharacterType != 'f')
											{
												expressionCharacterType = 'd';
											}
											
											break;
										
										case 'c':
											if(expressionCharacterType != 'f' && expressionCharacterType != 'd')
											{
												expressionCharacterType = 's';
											}
											
											break;
										
										case 's':
											if(expressionCharacterType != 'f' && expressionCharacterType != 'd' && expressionCharacterType != 's')
											{
												expressionCharacterType = 'c';
											}
											
											break;
									}
								}
							}
					
					break;
				
				default:					
					break;
			}
			
			expressionType(t -> first);
			expressionType(t -> second);
			expressionType(t -> third);
		}
	}
	
	void printCode(ternaryTree t)
	{
		if(t != NULL)
		{	
			switch(t -> nodeIdentifier)
			{
				case PROGRAM:
					printf("#include <stdio.h>\nvoid main(void)\n{\nregister int _by;\n");
					
					printCode(t -> second);
					
					printf("}\n");
					
					break;
				
				case BLOCK:
					printCode(t -> first);
					printCode(t -> second);
					
					break;
				
				case DECLARATION_LIST:
					printCode(t -> first);
					
					printf("\n");
					
					printCode(t -> second);
					
					break;
				
				case DECLARATION:
					printCode(t -> second);
					
					printf(" ");
					
					printCode(t -> first);
					
					printf(";");
					
					break;
				
				case TYPE_Y:
					switch(t -> item)
					{
						case REAL:
							identifierType = 'f';
							printf("float ");
							
							break;
						
						case INTEGER:
							identifierType = 'd';
							printf("int ");
							
							break;
						
						case CHARACTER:
							identifierType = 'c';
							printf("char ");
							
							break;
						
						default:
							break;
					}
					
					break;
				
				case STATEMENT_LIST:
					printCode(t -> first);				
					printCode(t -> second);
					
					break;
				
				case STATEMENT:
					printCode(t -> first);
					
					break;
				
				case ASSIGNMENT_STATEMENT:
					if(t -> item < 0 || t -> item > currentSymbolTableSize)
					{
						printf("Unknown Identifier: %d\n", t -> item);
						return;
					}
					else
					{
						printf("%s = ", symbolTable[t -> item] -> identifier);
					}
					
					if(t -> first -> first -> first -> item == CONSTANT && t -> first -> first -> first -> first -> item != NOTHING)
					{
						printf("'");
						
						printCode(t -> first);
						
						printf("'");
					}
					else
					{
						printCode(t -> first);
					}
					
					printf(";\n");
					
					break;
				
				case IF_STATEMENT:
					printf("if(");
					
					printCode(t -> first);
					
					printf(")\n{\n");
					
					printCode(t -> second);
					
					printf("}\n");
					
					if(t -> third != NULL)
					{
						printf("else\n{\n");
						
						printCode(t -> third);
						
						printf("}\n");
					}
					
					break;
				
				case DO_STATEMENT:
					printf("do\n{\n");
					
					printCode(t -> first);
					
					printf("} while(");
					
					printCode(t -> second);
					
					printf(");\n");
					
					break;
				
				case WHILE_STATEMENT:
					printf("while(");
					
					printCode(t -> first);
					
					printf(")\n{\n");
					
					printCode(t -> second);
					
					printf("}\n");
					
					break;
				
				case FOR_STATEMENT:
					if(t -> item < 0 || t -> item > currentSymbolTableSize)
					{
						printf("Unknown Identifier: %d\n", t -> item);
						return;
					}
					else
					{
						printf("for(%s = ", symbolTable[t -> item] -> identifier);
					}
					
					printCode(t -> first -> first);
					
					printf("; _by = ");
					
					printCode(t -> first -> second);
					
					if(t -> item < 0 || t -> item > currentSymbolTableSize)
					{
						printf("Unknown Identifier: %d\n", t -> item);
						return;
					}
					else
					{
						printf(", (%s - ", symbolTable[t -> item] -> identifier);
					}
					
					printCode(t -> first -> third);
					
					if(t -> item < 0 || t -> item > currentSymbolTableSize)
					{
						printf("Unknown Identifier: %d\n", t -> item);
						return;
					}
					else
					{
						printf(") * ((_by > 0) - (_by < 0)) <= 0; %s += _by)\n{\n", symbolTable[t -> item] -> identifier);
					}
					
					printCode(t -> second);
					
					printf("}\n");
					
					break;
				
				case WRITE_STATEMENT:				
					if(t -> first != NULL)
					{
						printCode(t -> first);
					}
					else
					{
						printf("printf(\"\\n\");\n");
					}
					
					break;
				
				case READ_STATEMENT:
					if(t -> item < 0 || t -> item > currentSymbolTableSize)
					{
						printf("Unknown Type: %d\n", t -> item);
						return;
					}
					else
					{
						if(symbolTable[t -> item] -> type == 'N')
						{
							printf("Unknown Type: %d\n", t -> item);
							return;
						}
						else
						{
							printf("scanf(\" %%%c\", &%s);\n", symbolTable[t -> item] -> type, symbolTable[t -> item] -> identifier);
						}
					}
					
					break;
				
				case OUTPUT_LIST:
					printf("printf(\"");
					
					if(t -> first -> item == EXPRESSION)
					{
						expressionType(t);
						
						if(expressionCharacterType == 'N')
						{
							return;
						}
						else
						{
							printf("%%%c\", ", expressionCharacterType);
						}
						
						expressionCharacterType == 'N';
					}
					else
					{
						if(t -> first -> first == NULL)
						{
							if(t -> first -> item < 0 || t -> first -> item > currentSymbolTableSize)
							{
								printf("Unknown Type: %d\n", t -> item);
								return;
							}
							else
							{
								if(symbolTable[t -> first -> item] -> type == 'N')
								{
									printf("Unknown Type: %d\n", t -> first -> item);
									return;
								}
								else
								{
									printf("%%%c\", ", symbolTable[t -> first -> item] -> type);
								}
							}
						}
					}
					
					printCode(t -> first);
					
					if(t -> first -> item != EXPRESSION && t -> first -> first != NULL)
					{
						printf("\"");
					}
					
					printf(");\n");
					
					printCode(t -> second);
					
					break;
				
				case CONDITIONAL_LIST:
					printCode(t -> first);
					
					if(t -> item == OR)
					{
						printf(" || ");
						
						printCode(t -> second);
					}
					else
					{
						if(t -> item == AND)
						{
							printf(" && ");
						
							printCode(t -> second);
						}
					}
					break;
				
				case CONDITIONAL:
					if(t -> second == NULL)
					{
						printf("!(");
						
						printCode(t -> first);
						
						printf(")");
					}
					else
					{
						printCode(t -> first);
						printCode(t -> second);
						printCode(t -> third);
					}
					
					break;
				
				case COMPARATOR:
					switch(t -> item)
					{
						case EQUAL:
							printf(" == ");
							
							break;
						
						case LESS_THAN:
							printf(" < ");
							
							break;
						
						case GREATER_THAN:
							printf(" > ");
							
							break;
						
						case LESS_THAN_EQUAL:
							printf(" <= ");
							
							break;
						
						case GREATER_THAN_EQUAL:
							printf(" >= ");
							
							break;
						
						case LESS_THAN_GREATER_THAN:
							printf(" != ");
							
							break;
						
						default:
							break;
					}
					
					break;
				
				case EXPRESSION:
					if(t -> second != NULL)
					{
						printf("(");
					
						printCode(t -> first);
						
						if(t -> item == PLUS)
						{
							printf(" + ");
						}
						else
						{
							printf(" - ");
						}
						
						printCode(t -> second);
						
						printf(")");
					}
					else
					{
						printCode(t -> first);
					}
					
					break;
				
				case TERM:
					if(t -> second != NULL)
					{
						printf("(");
					
						printCode(t -> first);
						
						if(t -> item == ASTERIX)
						{
							printf(" * ");
						}
						else
						{
							printf(" / ");
						}
						
						printCode(t -> second);
						
						printf(")");
					}
					else
					{
						printCode(t -> first);
					}
					
					break;
				
				case VALUE:
					if(t -> item != CONSTANT && t -> item != EXPRESSION)
					{
						if(t -> item < 0 || t -> item > currentSymbolTableSize)
						{
							printf("Unknown Identifier: %d\n", t -> item);
							return;
						}
						else
						{
							printf("%s", symbolTable[t -> item] -> identifier);
						}
					}
					else
					{
						printCode(t -> first);
					}
					
					break;
				
				case CONSTANT:
					if(t -> first == NULL)
					{
						printf("%c", t -> item);
					}
					else
					{
						printCode(t -> first);
					}
					
					break;
				
				case NUMBER_CONSTANT:
					printf("%d", t -> item);
					
					break;
				
				case NEGATIVE_NUMBER_CONSTANT:
					printf("-%d", t -> item);
					
					break;
				
				case FLOATING_NUMBER_CONSTANT:
					printf("%d.", t -> item);
					
					printCode(t -> first);
					
					break;
				
				case FLOATING_NEGATIVE_NUMBER_CONSTANT:
					printf("-%d.", t -> item);
					
					printCode(t -> first);
					
					break;
				
				case IDENTIFIER_LIST:
					if(t -> item < 0 || t -> item > currentSymbolTableSize)
					{
						printf("Unknown Identifier: %d\n", t -> item);
						return;
					}
					else
					{
						strcpy(underscore, constantUnderscore);
						strncat(underscore, symbolTable[t -> item] -> identifier, IDLENGTH);
						strcpy(symbolTable[t -> item] -> identifier, underscore);					
						symbolTable[t -> item] -> type = identifierType;
						printf("%s", symbolTable[t -> item] -> identifier, symbolTable[t -> item] -> type);
					}
					
					if(t -> first != NULL)
					{
						printf(", ");
						
						printCode(t -> first);
					}
					
					break;
				
				default:
					printCode(t -> first);
					printCode(t -> second);
					printCode(t -> third);
					
					break;
			}
		}
	}
#endif

#include "lex.yy.c"