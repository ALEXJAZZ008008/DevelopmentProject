%{
	#include <stdio.h>
	#include <stdlib.h>
	
	#define NOTHING -1
	
	struct treeNode
		{
			int  item;
			int  nodeIdentifier;
			struct treeNode * first;
			struct treeNode * second;
		};

	typedef struct treeNode TREE_NODE;
	typedef TREE_NODE * BINARY_TREE;
	
	void yyerror(char * s);
	int yylex(void);
	int evaluate(BINARY_TREE);
	BINARY_TREE create_node(int, int, BINARY_TREE, BINARY_TREE);
%}

%start lines

%union
	{
    		int iVal;
    		BINARY_TREE  tVal;
	}

%token<iVal> NUMBER PLUS TIMES BRA KET NEWLINE EXPR TERM
%type<tVal> line expr term factor

%%

line:	expr NEWLINE
		{
			BINARY_TREE ParseTree;
			int result;
			ParseTree = create_node(NOTHING, NEWLINE, $1, NULL);
			result = evaluate(ParseTree);
			printf("value : %d\n", result);
		};

lines:	line lines | line;

factor:	BRA expr KET
		{
			$$ = create_node(NOTHING, BRA, $2, NULL);
		}
	| NUMBER
		{
			$$ = create_node($1, NUMBER, NULL, NULL);
		};

term:	term TIMES  factor
		{
			$$ = create_node(NOTHING, TIMES, $1, $3);
		}
	| factor
		{
			$$ = create_node(NOTHING, TERM, $1, NULL);
		};

expr:	expr PLUS term
		{
			$$ = create_node(NOTHING, PLUS, $1, $3);
		}
	| term
		{
			$$ = create_node(NOTHING, EXPR, $1, NULL);
		};

%%

#include "lex.yy.c"

BINARY_TREE create_node(int ival, int case_identifier, BINARY_TREE p1, BINARY_TREE p2)
	{
		BINARY_TREE t;
		t = (BINARY_TREE)malloc(sizeof(TREE_NODE));
		t->item = ival;
		t->nodeIdentifier = case_identifier;
		t->first = p1;
		t->second = p2;
		return (t);
	}

int evaluate(BINARY_TREE t)
	{
		if (t != NULL)
			{
				switch(t->nodeIdentifier)
					{
						case(NEWLINE):
							return(evaluate(t -> first));
						
						case(PLUS):
							return((evaluate(t -> first)) + (evaluate(t -> second)));
						
						case(EXPR):
							return(evaluate(t -> first));
						
						case(TIMES):
							return((evaluate(t -> first)) * (evaluate(t -> second)));
						
						case(TERM):
							return(evaluate(t -> first));
						
						case(BRA):
							return(evaluate(t -> first));
						
						case(NUMBER):
							return(t -> item);
					}
    		}
	}