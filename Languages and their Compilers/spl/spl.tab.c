
/* A Bison parser, made by GNU Bison 2.4.1.  */

/* Skeleton implementation for Bison's Yacc-like parsers in C
   
      Copyright (C) 1984, 1989, 1990, 2000, 2001, 2002, 2003, 2004, 2005, 2006
   Free Software Foundation, Inc.
   
   This program is free software: you can redistribute it and/or modify
   it under the terms of the GNU General Public License as published by
   the Free Software Foundation, either version 3 of the License, or
   (at your option) any later version.
   
   This program is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU General Public License for more details.
   
   You should have received a copy of the GNU General Public License
   along with this program.  If not, see <http://www.gnu.org/licenses/>.  */

/* As a special exception, you may create a larger work that contains
   part or all of the Bison parser skeleton and distribute that work
   under terms of your choice, so long as that work isn't itself a
   parser generator using the skeleton or a modified version thereof
   as a parser skeleton.  Alternatively, if you modify or redistribute
   the parser skeleton itself, you may (at your option) remove this
   special exception, which will cause the skeleton and the resulting
   Bison output files to be licensed under the GNU General Public
   License without this special exception.
   
   This special exception was added by the Free Software Foundation in
   version 2.2 of Bison.  */

/* C LALR(1) parser skeleton written by Richard Stallman, by
   simplifying the original so-called "semantic" parser.  */

/* All symbols defined below should begin with yy or YY, to avoid
   infringing on user name space.  This should be done even for local
   variables, as they might otherwise be expanded by user macros.
   There are some unavoidable exceptions within include files to
   define necessary library symbols; they are noted "INFRINGES ON
   USER NAME SPACE" below.  */

/* Identify Bison output.  */
#define YYBISON 1

/* Bison version.  */
#define YYBISON_VERSION "2.4.1"

/* Skeleton name.  */
#define YYSKELETON_NAME "yacc.c"

/* Pure parsers.  */
#define YYPURE 0

/* Push parsers.  */
#define YYPUSH 0

/* Pull parsers.  */
#define YYPULL 1

/* Using locations.  */
#define YYLSP_NEEDED 0



/* Copy the first part of user declarations.  */

/* Line 189 of yacc.c  */
#line 1 "spl.y"
	
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
	void yyerror(char * error);
	
	enum parseTreeNodeType	{PROGRAM, BLOCK, DECLARATION_LIST, DECLARATION, TYPE_Y, STATEMENT_LIST, STATEMENT, ASSIGNMENT_STATEMENT, IF_STATEMENT, DO_STATEMENT, WHILE_STATEMENT, FOR_STATEMENT, WRITE_STATEMENT,
							READ_STATEMENT, OUTPUT_LIST, CONDITIONAL_LIST, CONDITIONAL, COMPARATOR, EXPRESSION, TERM, VALUE, CONSTANT, NUMBER_CONSTANT, NEGATIVE_NUMBER_CONSTANT, FLOATING_NUMBER_CONSTANT,
							FLOATING_NEGATIVE_NUMBER_CONSTANT, IDENTIFIER_LIST};
	
	char * nodeName[]	=	{"PROGRAM", "BLOCK", "DECLARATION_LIST", "DECLARATION", "TYPE_Y", "STATEMENT_LIST", "STATEMENT", "ASSIGNMENT_STATEMENT", "IF_STATEMENT", "DO_STATEMENT", "WHILE_STATEMENT", "FOR_STATEMENT",
							"WRITE_STATEMENT", "READ_STATEMENT", "OUTPUT_LIST", "CONDITIONAL_LIST", "CONDITIONAL", "COMPARATOR", "EXPRESSION", "TERM", "VALUE", "CONSTANT", "NUMBER_CONSTANT", "NEGATIVE_NUMBER_CONSTANT",
							"FLOATING_NUMBER_CONSTANT", "FLOATING_NEGATIVE_NUMBER_CONSTANT", "IDENTIFIER_LIST"};
	
	struct symbolTableNode
	{
		char identifier[IDLENGTH];
		char type;
		short flag;
	};
	
	struct treeNode
	{
		int item;
		int nodeIdentifier;
		struct treeNode * first;
		struct treeNode * second;
		struct treeNode * third;
	};
	
	typedef struct symbolTableNode symbolTableNode;
	typedef symbolTableNode * symbolTableNodePointer;
	symbolTableNodePointer symbolTable[SYMTABSIZE];
	
	typedef struct treeNode treeNode;
	typedef treeNode * ternaryTree;
	ternaryTree create_node(int, int, ternaryTree, ternaryTree, ternaryTree);
	
	int currentSymbolTableSize = 0;
	int forLoopInt = 0;
	
	short constantFoldingFlag, leftRightFlag, breakFlag;
	float floatLeft, floatRight;
	int intLeft, intRight;
	
	char constantUnderscore[IDLENGTH] = "_";
	char underscore[IDLENGTH];
	char identifierType = 'N';
	char expressionCharacterType = 'N';
	
	
	
	#ifdef DEBUG
		void printIdentifier(ternaryTree);
		void printTree(ternaryTree, int);
	#else
		void expressionType(ternaryTree);
		void floatConstantFolding(ternaryTree);
		void intConstantFolding(ternaryTree);
		void printCode(ternaryTree);
	#endif


/* Line 189 of yacc.c  */
#line 156 "spl.tab.c"

/* Enabling traces.  */
#ifndef YYDEBUG
# define YYDEBUG 0
#endif

/* Enabling verbose error messages.  */
#ifdef YYERROR_VERBOSE
# undef YYERROR_VERBOSE
# define YYERROR_VERBOSE 1
#else
# define YYERROR_VERBOSE 0
#endif

/* Enabling the token table.  */
#ifndef YYTOKEN_TABLE
# define YYTOKEN_TABLE 0
#endif


/* Tokens.  */
#ifndef YYTOKENTYPE
# define YYTOKENTYPE
   /* Put the tokens into the symbol table, so that GDB and other debuggers
      know about them.  */
   enum yytokentype {
     CHARACTER_CONSTANT = 258,
     NUMBER = 259,
     REAL_NUMBER = 260,
     IDENTIFIER = 261,
     COLON = 262,
     FULL_STOP = 263,
     COMMA = 264,
     SEMICOLON = 265,
     HYPHEN_GREATER_THAN = 266,
     OPEN_BRACKET = 267,
     CLOSE_BRACKET = 268,
     EQUAL = 269,
     LESS_THAN = 270,
     GREATER_THAN = 271,
     LESS_THAN_EQUAL = 272,
     GREATER_THAN_EQUAL = 273,
     LESS_THAN_GREATER_THAN = 274,
     PLUS = 275,
     HYPHEN = 276,
     ASTERIX = 277,
     FORWARD_SLASH = 278,
     ENDP = 279,
     CODE = 280,
     DECLARATIONS = 281,
     OF = 282,
     TYPE_L = 283,
     REAL = 284,
     INTEGER = 285,
     CHARACTER = 286,
     IF = 287,
     THEN = 288,
     ENDIF = 289,
     ELSE = 290,
     DO = 291,
     WHILE = 292,
     ENDDO = 293,
     ENDWHILE = 294,
     FOR = 295,
     IS = 296,
     BY = 297,
     TO = 298,
     ENDFOR = 299,
     WRITE = 300,
     NEWLINE = 301,
     READ = 302,
     NOT = 303,
     OR = 304,
     AND = 305
   };
#endif



#if ! defined YYSTYPE && ! defined YYSTYPE_IS_DECLARED
typedef union YYSTYPE
{

/* Line 214 of yacc.c  */
#line 86 "spl.y"

    int iVal;
    ternaryTree tVal;



/* Line 214 of yacc.c  */
#line 249 "spl.tab.c"
} YYSTYPE;
# define YYSTYPE_IS_TRIVIAL 1
# define yystype YYSTYPE /* obsolescent; will be withdrawn */
# define YYSTYPE_IS_DECLARED 1
#endif


/* Copy the second part of user declarations.  */


/* Line 264 of yacc.c  */
#line 261 "spl.tab.c"

#ifdef short
# undef short
#endif

#ifdef YYTYPE_UINT8
typedef YYTYPE_UINT8 yytype_uint8;
#else
typedef unsigned char yytype_uint8;
#endif

#ifdef YYTYPE_INT8
typedef YYTYPE_INT8 yytype_int8;
#elif (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
typedef signed char yytype_int8;
#else
typedef short int yytype_int8;
#endif

#ifdef YYTYPE_UINT16
typedef YYTYPE_UINT16 yytype_uint16;
#else
typedef unsigned short int yytype_uint16;
#endif

#ifdef YYTYPE_INT16
typedef YYTYPE_INT16 yytype_int16;
#else
typedef short int yytype_int16;
#endif

#ifndef YYSIZE_T
# ifdef __SIZE_TYPE__
#  define YYSIZE_T __SIZE_TYPE__
# elif defined size_t
#  define YYSIZE_T size_t
# elif ! defined YYSIZE_T && (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
#  include <stddef.h> /* INFRINGES ON USER NAME SPACE */
#  define YYSIZE_T size_t
# else
#  define YYSIZE_T unsigned int
# endif
#endif

#define YYSIZE_MAXIMUM ((YYSIZE_T) -1)

#ifndef YY_
# if YYENABLE_NLS
#  if ENABLE_NLS
#   include <libintl.h> /* INFRINGES ON USER NAME SPACE */
#   define YY_(msgid) dgettext ("bison-runtime", msgid)
#  endif
# endif
# ifndef YY_
#  define YY_(msgid) msgid
# endif
#endif

/* Suppress unused-variable warnings by "using" E.  */
#if ! defined lint || defined __GNUC__
# define YYUSE(e) ((void) (e))
#else
# define YYUSE(e) /* empty */
#endif

/* Identity function, used to suppress warnings about constant conditions.  */
#ifndef lint
# define YYID(n) (n)
#else
#if (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
static int
YYID (int yyi)
#else
static int
YYID (yyi)
    int yyi;
#endif
{
  return yyi;
}
#endif

#if ! defined yyoverflow || YYERROR_VERBOSE

/* The parser invokes alloca or malloc; define the necessary symbols.  */

# ifdef YYSTACK_USE_ALLOCA
#  if YYSTACK_USE_ALLOCA
#   ifdef __GNUC__
#    define YYSTACK_ALLOC __builtin_alloca
#   elif defined __BUILTIN_VA_ARG_INCR
#    include <alloca.h> /* INFRINGES ON USER NAME SPACE */
#   elif defined _AIX
#    define YYSTACK_ALLOC __alloca
#   elif defined _MSC_VER
#    include <malloc.h> /* INFRINGES ON USER NAME SPACE */
#    define alloca _alloca
#   else
#    define YYSTACK_ALLOC alloca
#    if ! defined _ALLOCA_H && ! defined _STDLIB_H && (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
#     include <stdlib.h> /* INFRINGES ON USER NAME SPACE */
#     ifndef _STDLIB_H
#      define _STDLIB_H 1
#     endif
#    endif
#   endif
#  endif
# endif

# ifdef YYSTACK_ALLOC
   /* Pacify GCC's `empty if-body' warning.  */
#  define YYSTACK_FREE(Ptr) do { /* empty */; } while (YYID (0))
#  ifndef YYSTACK_ALLOC_MAXIMUM
    /* The OS might guarantee only one guard page at the bottom of the stack,
       and a page size can be as small as 4096 bytes.  So we cannot safely
       invoke alloca (N) if N exceeds 4096.  Use a slightly smaller number
       to allow for a few compiler-allocated temporary stack slots.  */
#   define YYSTACK_ALLOC_MAXIMUM 4032 /* reasonable circa 2006 */
#  endif
# else
#  define YYSTACK_ALLOC YYMALLOC
#  define YYSTACK_FREE YYFREE
#  ifndef YYSTACK_ALLOC_MAXIMUM
#   define YYSTACK_ALLOC_MAXIMUM YYSIZE_MAXIMUM
#  endif
#  if (defined __cplusplus && ! defined _STDLIB_H \
       && ! ((defined YYMALLOC || defined malloc) \
	     && (defined YYFREE || defined free)))
#   include <stdlib.h> /* INFRINGES ON USER NAME SPACE */
#   ifndef _STDLIB_H
#    define _STDLIB_H 1
#   endif
#  endif
#  ifndef YYMALLOC
#   define YYMALLOC malloc
#   if ! defined malloc && ! defined _STDLIB_H && (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
void *malloc (YYSIZE_T); /* INFRINGES ON USER NAME SPACE */
#   endif
#  endif
#  ifndef YYFREE
#   define YYFREE free
#   if ! defined free && ! defined _STDLIB_H && (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
void free (void *); /* INFRINGES ON USER NAME SPACE */
#   endif
#  endif
# endif
#endif /* ! defined yyoverflow || YYERROR_VERBOSE */


#if (! defined yyoverflow \
     && (! defined __cplusplus \
	 || (defined YYSTYPE_IS_TRIVIAL && YYSTYPE_IS_TRIVIAL)))

/* A type that is properly aligned for any stack member.  */
union yyalloc
{
  yytype_int16 yyss_alloc;
  YYSTYPE yyvs_alloc;
};

/* The size of the maximum gap between one aligned stack and the next.  */
# define YYSTACK_GAP_MAXIMUM (sizeof (union yyalloc) - 1)

/* The size of an array large to enough to hold all stacks, each with
   N elements.  */
# define YYSTACK_BYTES(N) \
     ((N) * (sizeof (yytype_int16) + sizeof (YYSTYPE)) \
      + YYSTACK_GAP_MAXIMUM)

/* Copy COUNT objects from FROM to TO.  The source and destination do
   not overlap.  */
# ifndef YYCOPY
#  if defined __GNUC__ && 1 < __GNUC__
#   define YYCOPY(To, From, Count) \
      __builtin_memcpy (To, From, (Count) * sizeof (*(From)))
#  else
#   define YYCOPY(To, From, Count)		\
      do					\
	{					\
	  YYSIZE_T yyi;				\
	  for (yyi = 0; yyi < (Count); yyi++)	\
	    (To)[yyi] = (From)[yyi];		\
	}					\
      while (YYID (0))
#  endif
# endif

/* Relocate STACK from its old location to the new one.  The
   local variables YYSIZE and YYSTACKSIZE give the old and new number of
   elements in the stack, and YYPTR gives the new location of the
   stack.  Advance YYPTR to a properly aligned location for the next
   stack.  */
# define YYSTACK_RELOCATE(Stack_alloc, Stack)				\
    do									\
      {									\
	YYSIZE_T yynewbytes;						\
	YYCOPY (&yyptr->Stack_alloc, Stack, yysize);			\
	Stack = &yyptr->Stack_alloc;					\
	yynewbytes = yystacksize * sizeof (*Stack) + YYSTACK_GAP_MAXIMUM; \
	yyptr += yynewbytes / sizeof (*yyptr);				\
      }									\
    while (YYID (0))

#endif

/* YYFINAL -- State number of the termination state.  */
#define YYFINAL  4
/* YYLAST -- Last index in YYTABLE.  */
#define YYLAST   113

/* YYNTOKENS -- Number of terminals.  */
#define YYNTOKENS  51
/* YYNNTS -- Number of nonterminals.  */
#define YYNNTS  25
/* YYNRULES -- Number of rules.  */
#define YYNRULES  58
/* YYNRULES -- Number of states.  */
#define YYNSTATES  120

/* YYTRANSLATE(YYLEX) -- Bison symbol number corresponding to YYLEX.  */
#define YYUNDEFTOK  2
#define YYMAXUTOK   305

#define YYTRANSLATE(YYX)						\
  ((unsigned int) (YYX) <= YYMAXUTOK ? yytranslate[YYX] : YYUNDEFTOK)

/* YYTRANSLATE[YYLEX] -- Bison symbol number corresponding to YYLEX.  */
static const yytype_uint8 yytranslate[] =
{
       0,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     2,     2,     2,     2,
       2,     2,     2,     2,     2,     2,     1,     2,     3,     4,
       5,     6,     7,     8,     9,    10,    11,    12,    13,    14,
      15,    16,    17,    18,    19,    20,    21,    22,    23,    24,
      25,    26,    27,    28,    29,    30,    31,    32,    33,    34,
      35,    36,    37,    38,    39,    40,    41,    42,    43,    44,
      45,    46,    47,    48,    49,    50
};

#if YYDEBUG
/* YYPRHS[YYN] -- Index of the first RHS symbol of rule number YYN in
   YYRHS.  */
static const yytype_uint8 yyprhs[] =
{
       0,     0,     3,    10,    13,    18,    20,    23,    29,    31,
      33,    35,    37,    41,    43,    45,    47,    49,    51,    53,
      55,    59,    65,    73,    79,    85,    97,    99,   104,   109,
     111,   115,   117,   121,   125,   128,   132,   134,   136,   138,
     140,   142,   144,   146,   150,   154,   156,   160,   164,   166,
     168,   172,   174,   176,   178,   181,   183,   186,   188
};

/* YYRHS -- A `-1'-separated list of the rules' RHS.  */
static const yytype_int8 yyrhs[] =
{
      52,     0,    -1,     6,     7,    53,    24,     6,     8,    -1,
      25,    57,    -1,    26,    54,    25,    57,    -1,    55,    -1,
      55,    54,    -1,    75,    27,    28,    56,    10,    -1,    29,
      -1,    30,    -1,    31,    -1,    58,    -1,    58,    10,    57,
      -1,    59,    -1,    60,    -1,    61,    -1,    62,    -1,    63,
      -1,    64,    -1,    65,    -1,    70,    11,     6,    -1,    32,
      67,    33,    57,    34,    -1,    32,    67,    33,    57,    35,
      57,    34,    -1,    36,    57,    37,    67,    38,    -1,    37,
      67,    36,    57,    39,    -1,    40,     6,    41,    70,    42,
      70,    43,    70,    36,    57,    44,    -1,    46,    -1,    45,
      12,    66,    13,    -1,    47,    12,     6,    13,    -1,    72,
      -1,    72,     9,    66,    -1,    68,    -1,    68,    49,    68,
      -1,    68,    50,    68,    -1,    48,    68,    -1,    70,    69,
      70,    -1,    14,    -1,    15,    -1,    16,    -1,    17,    -1,
      18,    -1,    19,    -1,    71,    -1,    71,    20,    70,    -1,
      71,    21,    70,    -1,    72,    -1,    72,    22,    71,    -1,
      72,    23,    71,    -1,    73,    -1,     6,    -1,    12,    70,
      13,    -1,    74,    -1,     3,    -1,     4,    -1,    21,     4,
      -1,     5,    -1,    21,     5,    -1,     6,    -1,    75,     9,
       6,    -1
};

/* YYRLINE[YYN] -- source line where rule number YYN was defined.  */
static const yytype_uint16 yyrline[] =
{
       0,   101,   101,   121,   125,   131,   135,   141,   147,   151,
     155,   161,   165,   171,   175,   179,   183,   187,   191,   195,
     201,   207,   211,   217,   223,   229,   235,   239,   245,   251,
     255,   261,   265,   269,   275,   279,   284,   288,   292,   296,
     300,   304,   310,   314,   318,   324,   328,   332,   338,   342,
     346,   352,   356,   362,   366,   370,   374,   380,   384
};
#endif

#if YYDEBUG || YYERROR_VERBOSE || YYTOKEN_TABLE
/* YYTNAME[SYMBOL-NUM] -- String name of the symbol SYMBOL-NUM.
   First, the terminals, then, starting at YYNTOKENS, nonterminals.  */
static const char *const yytname[] =
{
  "$end", "error", "$undefined", "CHARACTER_CONSTANT", "NUMBER",
  "REAL_NUMBER", "IDENTIFIER", "COLON", "FULL_STOP", "COMMA", "SEMICOLON",
  "HYPHEN_GREATER_THAN", "OPEN_BRACKET", "CLOSE_BRACKET", "EQUAL",
  "LESS_THAN", "GREATER_THAN", "LESS_THAN_EQUAL", "GREATER_THAN_EQUAL",
  "LESS_THAN_GREATER_THAN", "PLUS", "HYPHEN", "ASTERIX", "FORWARD_SLASH",
  "ENDP", "CODE", "DECLARATIONS", "OF", "TYPE_L", "REAL", "INTEGER",
  "CHARACTER", "IF", "THEN", "ENDIF", "ELSE", "DO", "WHILE", "ENDDO",
  "ENDWHILE", "FOR", "IS", "BY", "TO", "ENDFOR", "WRITE", "NEWLINE",
  "READ", "NOT", "OR", "AND", "$accept", "program", "block",
  "declaration_list", "declaration", "type_y", "statement_list",
  "statement", "assignment_statement", "if_statement", "do_statement",
  "while_statement", "for_statement", "write_statement", "read_statement",
  "output_list", "conditional_list", "conditional", "comparator",
  "expression", "term", "value", "constant", "number_constant",
  "identifier_list", 0
};
#endif

# ifdef YYPRINT
/* YYTOKNUM[YYLEX-NUM] -- Internal token number corresponding to
   token YYLEX-NUM.  */
static const yytype_uint16 yytoknum[] =
{
       0,   256,   257,   258,   259,   260,   261,   262,   263,   264,
     265,   266,   267,   268,   269,   270,   271,   272,   273,   274,
     275,   276,   277,   278,   279,   280,   281,   282,   283,   284,
     285,   286,   287,   288,   289,   290,   291,   292,   293,   294,
     295,   296,   297,   298,   299,   300,   301,   302,   303,   304,
     305
};
# endif

/* YYR1[YYN] -- Symbol number of symbol that rule YYN derives.  */
static const yytype_uint8 yyr1[] =
{
       0,    51,    52,    53,    53,    54,    54,    55,    56,    56,
      56,    57,    57,    58,    58,    58,    58,    58,    58,    58,
      59,    60,    60,    61,    62,    63,    64,    64,    65,    66,
      66,    67,    67,    67,    68,    68,    69,    69,    69,    69,
      69,    69,    70,    70,    70,    71,    71,    71,    72,    72,
      72,    73,    73,    74,    74,    74,    74,    75,    75
};

/* YYR2[YYN] -- Number of symbols composing right hand side of rule YYN.  */
static const yytype_uint8 yyr2[] =
{
       0,     2,     6,     2,     4,     1,     2,     5,     1,     1,
       1,     1,     3,     1,     1,     1,     1,     1,     1,     1,
       3,     5,     7,     5,     5,    11,     1,     4,     4,     1,
       3,     1,     3,     3,     2,     3,     1,     1,     1,     1,
       1,     1,     1,     3,     3,     1,     3,     3,     1,     1,
       3,     1,     1,     1,     2,     1,     2,     1,     3
};

/* YYDEFACT[STATE-NAME] -- Default rule to reduce with in state
   STATE-NUM when YYTABLE doesn't specify something else to do.  Zero
   means the default is an error.  */
static const yytype_uint8 yydefact[] =
{
       0,     0,     0,     0,     1,     0,     0,     0,    52,    53,
      55,    49,     0,     0,     0,     0,     0,     0,     0,    26,
       0,     3,    11,    13,    14,    15,    16,    17,    18,    19,
       0,    42,    45,    48,    51,    57,     0,     5,     0,     0,
       0,    54,    56,     0,     0,    31,     0,     0,     0,     0,
       0,     0,     0,     0,     0,     0,     0,     0,     0,     6,
       0,     0,     0,    50,    34,     0,     0,     0,    36,    37,
      38,    39,    40,    41,     0,     0,     0,     0,     0,    29,
       0,    12,    20,    43,    44,    46,    47,     4,    58,     0,
       2,     0,    32,    33,    35,     0,     0,     0,    27,     0,
      28,     8,     9,    10,     0,    21,     0,    23,    24,     0,
      30,     7,     0,     0,    22,     0,     0,     0,     0,    25
};

/* YYDEFGOTO[NTERM-NUM].  */
static const yytype_int8 yydefgoto[] =
{
      -1,     2,     7,    36,    37,   104,    21,    22,    23,    24,
      25,    26,    27,    28,    29,    78,    44,    45,    74,    30,
      31,    32,    33,    34,    38
};

/* YYPACT[STATE-NUM] -- Index in YYTABLE of the portion describing
   STATE-NUM.  */
#define YYPACT_NINF -48
static const yytype_int8 yypact[] =
{
      10,    54,    25,    20,   -48,    35,    21,    27,   -48,   -48,
     -48,   -48,     9,    44,     5,    35,     5,    58,    66,   -48,
      67,   -48,    56,   -48,   -48,   -48,   -48,   -48,   -48,   -48,
      72,     2,    36,   -48,   -48,   -48,    59,    21,    -3,    79,
      73,   -48,   -48,     5,    55,    24,    18,    50,    53,    49,
       9,    85,    35,    86,     9,     9,     9,     9,    35,   -48,
      87,    68,    91,   -48,   -48,    35,     5,     5,   -48,   -48,
     -48,   -48,   -48,   -48,     9,     5,    35,     9,    81,    92,
      82,   -48,   -48,   -48,   -48,   -48,   -48,   -48,   -48,   -11,
     -48,    42,   -48,   -48,   -48,    62,    63,    64,   -48,     9,
     -48,   -48,   -48,   -48,    94,   -48,    35,   -48,   -48,     9,
     -48,   -48,    71,    65,   -48,     9,    74,    35,    69,   -48
};

/* YYPGOTO[NTERM-NUM].  */
static const yytype_int8 yypgoto[] =
{
     -48,   -48,   -48,    70,   -48,   -48,    -8,   -48,   -48,   -48,
     -48,   -48,   -48,   -48,   -48,    12,   -15,   -38,   -48,   -12,
      13,   -47,   -48,   -48,   -48
};

/* YYTABLE[YYPACT[STATE-NUM]].  What to do in state STATE-NUM.  If
   positive, shift that token.  If negative, reduce the rule which
   number is the opposite.  If zero, do what YYDEFACT says.
   If YYTABLE_NINF, syntax error.  */
#define YYTABLE_NINF -1
static const yytype_uint8 yytable[] =
{
      40,    48,    46,    79,    46,    64,    60,    47,     8,     9,
      10,    11,     8,     9,    10,    11,     1,    12,   101,   102,
     103,    12,    54,    55,    61,     4,    13,    35,    92,    93,
      13,    46,    68,    69,    70,    71,    72,    73,     8,     9,
      10,    11,    83,    84,    81,     5,     6,    12,    41,    42,
      87,    39,    79,    43,    46,    46,    13,    91,    56,    57,
      95,     3,    94,    46,    49,    97,    52,    14,    96,    85,
      86,    15,    16,    66,    67,    17,   105,   106,    50,    51,
      18,    19,    20,    53,    58,    62,    63,    75,    65,    76,
      77,    80,    82,    88,    98,   100,    89,   113,   112,    90,
     107,    99,   108,   116,   111,   114,   109,    59,   115,   118,
     117,   110,     0,   119
};

static const yytype_int8 yycheck[] =
{
      12,    16,    14,    50,    16,    43,     9,    15,     3,     4,
       5,     6,     3,     4,     5,     6,     6,    12,    29,    30,
      31,    12,    20,    21,    27,     0,    21,     6,    66,    67,
      21,    43,    14,    15,    16,    17,    18,    19,     3,     4,
       5,     6,    54,    55,    52,    25,    26,    12,     4,     5,
      58,    24,    99,    48,    66,    67,    21,    65,    22,    23,
      75,     7,    74,    75,     6,    77,    10,    32,    76,    56,
      57,    36,    37,    49,    50,    40,    34,    35,    12,    12,
      45,    46,    47,    11,    25,     6,    13,    37,    33,    36,
      41,     6,     6,     6,    13,    13,    28,   109,   106,     8,
      38,     9,    39,   115,    10,    34,    42,    37,    43,   117,
      36,    99,    -1,    44
};

/* YYSTOS[STATE-NUM] -- The (internal number of the) accessing
   symbol of state STATE-NUM.  */
static const yytype_uint8 yystos[] =
{
       0,     6,    52,     7,     0,    25,    26,    53,     3,     4,
       5,     6,    12,    21,    32,    36,    37,    40,    45,    46,
      47,    57,    58,    59,    60,    61,    62,    63,    64,    65,
      70,    71,    72,    73,    74,     6,    54,    55,    75,    24,
      70,     4,     5,    48,    67,    68,    70,    57,    67,     6,
      12,    12,    10,    11,    20,    21,    22,    23,    25,    54,
       9,    27,     6,    13,    68,    33,    49,    50,    14,    15,
      16,    17,    18,    19,    69,    37,    36,    41,    66,    72,
       6,    57,     6,    70,    70,    71,    71,    57,     6,    28,
       8,    57,    68,    68,    70,    67,    57,    70,    13,     9,
      13,    29,    30,    31,    56,    34,    35,    38,    39,    42,
      66,    10,    57,    70,    34,    43,    70,    36,    57,    44
};

#define yyerrok		(yyerrstatus = 0)
#define yyclearin	(yychar = YYEMPTY)
#define YYEMPTY		(-2)
#define YYEOF		0

#define YYACCEPT	goto yyacceptlab
#define YYABORT		goto yyabortlab
#define YYERROR		goto yyerrorlab


/* Like YYERROR except do call yyerror.  This remains here temporarily
   to ease the transition to the new meaning of YYERROR, for GCC.
   Once GCC version 2 has supplanted version 1, this can go.  */

#define YYFAIL		goto yyerrlab

#define YYRECOVERING()  (!!yyerrstatus)

#define YYBACKUP(Token, Value)					\
do								\
  if (yychar == YYEMPTY && yylen == 1)				\
    {								\
      yychar = (Token);						\
      yylval = (Value);						\
      yytoken = YYTRANSLATE (yychar);				\
      YYPOPSTACK (1);						\
      goto yybackup;						\
    }								\
  else								\
    {								\
      yyerror (YY_("syntax error: cannot back up")); \
      YYERROR;							\
    }								\
while (YYID (0))


#define YYTERROR	1
#define YYERRCODE	256


/* YYLLOC_DEFAULT -- Set CURRENT to span from RHS[1] to RHS[N].
   If N is 0, then set CURRENT to the empty location which ends
   the previous symbol: RHS[0] (always defined).  */

#define YYRHSLOC(Rhs, K) ((Rhs)[K])
#ifndef YYLLOC_DEFAULT
# define YYLLOC_DEFAULT(Current, Rhs, N)				\
    do									\
      if (YYID (N))                                                    \
	{								\
	  (Current).first_line   = YYRHSLOC (Rhs, 1).first_line;	\
	  (Current).first_column = YYRHSLOC (Rhs, 1).first_column;	\
	  (Current).last_line    = YYRHSLOC (Rhs, N).last_line;		\
	  (Current).last_column  = YYRHSLOC (Rhs, N).last_column;	\
	}								\
      else								\
	{								\
	  (Current).first_line   = (Current).last_line   =		\
	    YYRHSLOC (Rhs, 0).last_line;				\
	  (Current).first_column = (Current).last_column =		\
	    YYRHSLOC (Rhs, 0).last_column;				\
	}								\
    while (YYID (0))
#endif


/* YY_LOCATION_PRINT -- Print the location on the stream.
   This macro was not mandated originally: define only if we know
   we won't break user code: when these are the locations we know.  */

#ifndef YY_LOCATION_PRINT
# if YYLTYPE_IS_TRIVIAL
#  define YY_LOCATION_PRINT(File, Loc)			\
     fprintf (File, "%d.%d-%d.%d",			\
	      (Loc).first_line, (Loc).first_column,	\
	      (Loc).last_line,  (Loc).last_column)
# else
#  define YY_LOCATION_PRINT(File, Loc) ((void) 0)
# endif
#endif


/* YYLEX -- calling `yylex' with the right arguments.  */

#ifdef YYLEX_PARAM
# define YYLEX yylex (YYLEX_PARAM)
#else
# define YYLEX yylex ()
#endif

/* Enable debugging if requested.  */
#if YYDEBUG

# ifndef YYFPRINTF
#  include <stdio.h> /* INFRINGES ON USER NAME SPACE */
#  define YYFPRINTF fprintf
# endif

# define YYDPRINTF(Args)			\
do {						\
  if (yydebug)					\
    YYFPRINTF Args;				\
} while (YYID (0))

# define YY_SYMBOL_PRINT(Title, Type, Value, Location)			  \
do {									  \
  if (yydebug)								  \
    {									  \
      YYFPRINTF (stderr, "%s ", Title);					  \
      yy_symbol_print (stderr,						  \
		  Type, Value); \
      YYFPRINTF (stderr, "\n");						  \
    }									  \
} while (YYID (0))


/*--------------------------------.
| Print this symbol on YYOUTPUT.  |
`--------------------------------*/

/*ARGSUSED*/
#if (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
static void
yy_symbol_value_print (FILE *yyoutput, int yytype, YYSTYPE const * const yyvaluep)
#else
static void
yy_symbol_value_print (yyoutput, yytype, yyvaluep)
    FILE *yyoutput;
    int yytype;
    YYSTYPE const * const yyvaluep;
#endif
{
  if (!yyvaluep)
    return;
# ifdef YYPRINT
  if (yytype < YYNTOKENS)
    YYPRINT (yyoutput, yytoknum[yytype], *yyvaluep);
# else
  YYUSE (yyoutput);
# endif
  switch (yytype)
    {
      default:
	break;
    }
}


/*--------------------------------.
| Print this symbol on YYOUTPUT.  |
`--------------------------------*/

#if (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
static void
yy_symbol_print (FILE *yyoutput, int yytype, YYSTYPE const * const yyvaluep)
#else
static void
yy_symbol_print (yyoutput, yytype, yyvaluep)
    FILE *yyoutput;
    int yytype;
    YYSTYPE const * const yyvaluep;
#endif
{
  if (yytype < YYNTOKENS)
    YYFPRINTF (yyoutput, "token %s (", yytname[yytype]);
  else
    YYFPRINTF (yyoutput, "nterm %s (", yytname[yytype]);

  yy_symbol_value_print (yyoutput, yytype, yyvaluep);
  YYFPRINTF (yyoutput, ")");
}

/*------------------------------------------------------------------.
| yy_stack_print -- Print the state stack from its BOTTOM up to its |
| TOP (included).                                                   |
`------------------------------------------------------------------*/

#if (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
static void
yy_stack_print (yytype_int16 *yybottom, yytype_int16 *yytop)
#else
static void
yy_stack_print (yybottom, yytop)
    yytype_int16 *yybottom;
    yytype_int16 *yytop;
#endif
{
  YYFPRINTF (stderr, "Stack now");
  for (; yybottom <= yytop; yybottom++)
    {
      int yybot = *yybottom;
      YYFPRINTF (stderr, " %d", yybot);
    }
  YYFPRINTF (stderr, "\n");
}

# define YY_STACK_PRINT(Bottom, Top)				\
do {								\
  if (yydebug)							\
    yy_stack_print ((Bottom), (Top));				\
} while (YYID (0))


/*------------------------------------------------.
| Report that the YYRULE is going to be reduced.  |
`------------------------------------------------*/

#if (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
static void
yy_reduce_print (YYSTYPE *yyvsp, int yyrule)
#else
static void
yy_reduce_print (yyvsp, yyrule)
    YYSTYPE *yyvsp;
    int yyrule;
#endif
{
  int yynrhs = yyr2[yyrule];
  int yyi;
  unsigned long int yylno = yyrline[yyrule];
  YYFPRINTF (stderr, "Reducing stack by rule %d (line %lu):\n",
	     yyrule - 1, yylno);
  /* The symbols being reduced.  */
  for (yyi = 0; yyi < yynrhs; yyi++)
    {
      YYFPRINTF (stderr, "   $%d = ", yyi + 1);
      yy_symbol_print (stderr, yyrhs[yyprhs[yyrule] + yyi],
		       &(yyvsp[(yyi + 1) - (yynrhs)])
		       		       );
      YYFPRINTF (stderr, "\n");
    }
}

# define YY_REDUCE_PRINT(Rule)		\
do {					\
  if (yydebug)				\
    yy_reduce_print (yyvsp, Rule); \
} while (YYID (0))

/* Nonzero means print parse trace.  It is left uninitialized so that
   multiple parsers can coexist.  */
int yydebug;
#else /* !YYDEBUG */
# define YYDPRINTF(Args)
# define YY_SYMBOL_PRINT(Title, Type, Value, Location)
# define YY_STACK_PRINT(Bottom, Top)
# define YY_REDUCE_PRINT(Rule)
#endif /* !YYDEBUG */


/* YYINITDEPTH -- initial size of the parser's stacks.  */
#ifndef	YYINITDEPTH
# define YYINITDEPTH 200
#endif

/* YYMAXDEPTH -- maximum size the stacks can grow to (effective only
   if the built-in stack extension method is used).

   Do not make this value too large; the results are undefined if
   YYSTACK_ALLOC_MAXIMUM < YYSTACK_BYTES (YYMAXDEPTH)
   evaluated with infinite-precision integer arithmetic.  */

#ifndef YYMAXDEPTH
# define YYMAXDEPTH 10000
#endif



#if YYERROR_VERBOSE

# ifndef yystrlen
#  if defined __GLIBC__ && defined _STRING_H
#   define yystrlen strlen
#  else
/* Return the length of YYSTR.  */
#if (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
static YYSIZE_T
yystrlen (const char *yystr)
#else
static YYSIZE_T
yystrlen (yystr)
    const char *yystr;
#endif
{
  YYSIZE_T yylen;
  for (yylen = 0; yystr[yylen]; yylen++)
    continue;
  return yylen;
}
#  endif
# endif

# ifndef yystpcpy
#  if defined __GLIBC__ && defined _STRING_H && defined _GNU_SOURCE
#   define yystpcpy stpcpy
#  else
/* Copy YYSRC to YYDEST, returning the address of the terminating '\0' in
   YYDEST.  */
#if (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
static char *
yystpcpy (char *yydest, const char *yysrc)
#else
static char *
yystpcpy (yydest, yysrc)
    char *yydest;
    const char *yysrc;
#endif
{
  char *yyd = yydest;
  const char *yys = yysrc;

  while ((*yyd++ = *yys++) != '\0')
    continue;

  return yyd - 1;
}
#  endif
# endif

# ifndef yytnamerr
/* Copy to YYRES the contents of YYSTR after stripping away unnecessary
   quotes and backslashes, so that it's suitable for yyerror.  The
   heuristic is that double-quoting is unnecessary unless the string
   contains an apostrophe, a comma, or backslash (other than
   backslash-backslash).  YYSTR is taken from yytname.  If YYRES is
   null, do not copy; instead, return the length of what the result
   would have been.  */
static YYSIZE_T
yytnamerr (char *yyres, const char *yystr)
{
  if (*yystr == '"')
    {
      YYSIZE_T yyn = 0;
      char const *yyp = yystr;

      for (;;)
	switch (*++yyp)
	  {
	  case '\'':
	  case ',':
	    goto do_not_strip_quotes;

	  case '\\':
	    if (*++yyp != '\\')
	      goto do_not_strip_quotes;
	    /* Fall through.  */
	  default:
	    if (yyres)
	      yyres[yyn] = *yyp;
	    yyn++;
	    break;

	  case '"':
	    if (yyres)
	      yyres[yyn] = '\0';
	    return yyn;
	  }
    do_not_strip_quotes: ;
    }

  if (! yyres)
    return yystrlen (yystr);

  return yystpcpy (yyres, yystr) - yyres;
}
# endif

/* Copy into YYRESULT an error message about the unexpected token
   YYCHAR while in state YYSTATE.  Return the number of bytes copied,
   including the terminating null byte.  If YYRESULT is null, do not
   copy anything; just return the number of bytes that would be
   copied.  As a special case, return 0 if an ordinary "syntax error"
   message will do.  Return YYSIZE_MAXIMUM if overflow occurs during
   size calculation.  */
static YYSIZE_T
yysyntax_error (char *yyresult, int yystate, int yychar)
{
  int yyn = yypact[yystate];

  if (! (YYPACT_NINF < yyn && yyn <= YYLAST))
    return 0;
  else
    {
      int yytype = YYTRANSLATE (yychar);
      YYSIZE_T yysize0 = yytnamerr (0, yytname[yytype]);
      YYSIZE_T yysize = yysize0;
      YYSIZE_T yysize1;
      int yysize_overflow = 0;
      enum { YYERROR_VERBOSE_ARGS_MAXIMUM = 5 };
      char const *yyarg[YYERROR_VERBOSE_ARGS_MAXIMUM];
      int yyx;

# if 0
      /* This is so xgettext sees the translatable formats that are
	 constructed on the fly.  */
      YY_("syntax error, unexpected %s");
      YY_("syntax error, unexpected %s, expecting %s");
      YY_("syntax error, unexpected %s, expecting %s or %s");
      YY_("syntax error, unexpected %s, expecting %s or %s or %s");
      YY_("syntax error, unexpected %s, expecting %s or %s or %s or %s");
# endif
      char *yyfmt;
      char const *yyf;
      static char const yyunexpected[] = "syntax error, unexpected %s";
      static char const yyexpecting[] = ", expecting %s";
      static char const yyor[] = " or %s";
      char yyformat[sizeof yyunexpected
		    + sizeof yyexpecting - 1
		    + ((YYERROR_VERBOSE_ARGS_MAXIMUM - 2)
		       * (sizeof yyor - 1))];
      char const *yyprefix = yyexpecting;

      /* Start YYX at -YYN if negative to avoid negative indexes in
	 YYCHECK.  */
      int yyxbegin = yyn < 0 ? -yyn : 0;

      /* Stay within bounds of both yycheck and yytname.  */
      int yychecklim = YYLAST - yyn + 1;
      int yyxend = yychecklim < YYNTOKENS ? yychecklim : YYNTOKENS;
      int yycount = 1;

      yyarg[0] = yytname[yytype];
      yyfmt = yystpcpy (yyformat, yyunexpected);

      for (yyx = yyxbegin; yyx < yyxend; ++yyx)
	if (yycheck[yyx + yyn] == yyx && yyx != YYTERROR)
	  {
	    if (yycount == YYERROR_VERBOSE_ARGS_MAXIMUM)
	      {
		yycount = 1;
		yysize = yysize0;
		yyformat[sizeof yyunexpected - 1] = '\0';
		break;
	      }
	    yyarg[yycount++] = yytname[yyx];
	    yysize1 = yysize + yytnamerr (0, yytname[yyx]);
	    yysize_overflow |= (yysize1 < yysize);
	    yysize = yysize1;
	    yyfmt = yystpcpy (yyfmt, yyprefix);
	    yyprefix = yyor;
	  }

      yyf = YY_(yyformat);
      yysize1 = yysize + yystrlen (yyf);
      yysize_overflow |= (yysize1 < yysize);
      yysize = yysize1;

      if (yysize_overflow)
	return YYSIZE_MAXIMUM;

      if (yyresult)
	{
	  /* Avoid sprintf, as that infringes on the user's name space.
	     Don't have undefined behavior even if the translation
	     produced a string with the wrong number of "%s"s.  */
	  char *yyp = yyresult;
	  int yyi = 0;
	  while ((*yyp = *yyf) != '\0')
	    {
	      if (*yyp == '%' && yyf[1] == 's' && yyi < yycount)
		{
		  yyp += yytnamerr (yyp, yyarg[yyi++]);
		  yyf += 2;
		}
	      else
		{
		  yyp++;
		  yyf++;
		}
	    }
	}
      return yysize;
    }
}
#endif /* YYERROR_VERBOSE */


/*-----------------------------------------------.
| Release the memory associated to this symbol.  |
`-----------------------------------------------*/

/*ARGSUSED*/
#if (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
static void
yydestruct (const char *yymsg, int yytype, YYSTYPE *yyvaluep)
#else
static void
yydestruct (yymsg, yytype, yyvaluep)
    const char *yymsg;
    int yytype;
    YYSTYPE *yyvaluep;
#endif
{
  YYUSE (yyvaluep);

  if (!yymsg)
    yymsg = "Deleting";
  YY_SYMBOL_PRINT (yymsg, yytype, yyvaluep, yylocationp);

  switch (yytype)
    {

      default:
	break;
    }
}

/* Prevent warnings from -Wmissing-prototypes.  */
#ifdef YYPARSE_PARAM
#if defined __STDC__ || defined __cplusplus
int yyparse (void *YYPARSE_PARAM);
#else
int yyparse ();
#endif
#else /* ! YYPARSE_PARAM */
#if defined __STDC__ || defined __cplusplus
int yyparse (void);
#else
int yyparse ();
#endif
#endif /* ! YYPARSE_PARAM */


/* The lookahead symbol.  */
int yychar;

/* The semantic value of the lookahead symbol.  */
YYSTYPE yylval;

/* Number of syntax errors so far.  */
int yynerrs;



/*-------------------------.
| yyparse or yypush_parse.  |
`-------------------------*/

#ifdef YYPARSE_PARAM
#if (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
int
yyparse (void *YYPARSE_PARAM)
#else
int
yyparse (YYPARSE_PARAM)
    void *YYPARSE_PARAM;
#endif
#else /* ! YYPARSE_PARAM */
#if (defined __STDC__ || defined __C99__FUNC__ \
     || defined __cplusplus || defined _MSC_VER)
int
yyparse (void)
#else
int
yyparse ()

#endif
#endif
{


    int yystate;
    /* Number of tokens to shift before error messages enabled.  */
    int yyerrstatus;

    /* The stacks and their tools:
       `yyss': related to states.
       `yyvs': related to semantic values.

       Refer to the stacks thru separate pointers, to allow yyoverflow
       to reallocate them elsewhere.  */

    /* The state stack.  */
    yytype_int16 yyssa[YYINITDEPTH];
    yytype_int16 *yyss;
    yytype_int16 *yyssp;

    /* The semantic value stack.  */
    YYSTYPE yyvsa[YYINITDEPTH];
    YYSTYPE *yyvs;
    YYSTYPE *yyvsp;

    YYSIZE_T yystacksize;

  int yyn;
  int yyresult;
  /* Lookahead token as an internal (translated) token number.  */
  int yytoken;
  /* The variables used to return semantic value and location from the
     action routines.  */
  YYSTYPE yyval;

#if YYERROR_VERBOSE
  /* Buffer for error messages, and its allocated size.  */
  char yymsgbuf[128];
  char *yymsg = yymsgbuf;
  YYSIZE_T yymsg_alloc = sizeof yymsgbuf;
#endif

#define YYPOPSTACK(N)   (yyvsp -= (N), yyssp -= (N))

  /* The number of symbols on the RHS of the reduced rule.
     Keep to zero when no symbol should be popped.  */
  int yylen = 0;

  yytoken = 0;
  yyss = yyssa;
  yyvs = yyvsa;
  yystacksize = YYINITDEPTH;

  YYDPRINTF ((stderr, "Starting parse\n"));

  yystate = 0;
  yyerrstatus = 0;
  yynerrs = 0;
  yychar = YYEMPTY; /* Cause a token to be read.  */

  /* Initialize stack pointers.
     Waste one element of value and location stack
     so that they stay on the same level as the state stack.
     The wasted elements are never initialized.  */
  yyssp = yyss;
  yyvsp = yyvs;

  goto yysetstate;

/*------------------------------------------------------------.
| yynewstate -- Push a new state, which is found in yystate.  |
`------------------------------------------------------------*/
 yynewstate:
  /* In all cases, when you get here, the value and location stacks
     have just been pushed.  So pushing a state here evens the stacks.  */
  yyssp++;

 yysetstate:
  *yyssp = yystate;

  if (yyss + yystacksize - 1 <= yyssp)
    {
      /* Get the current used size of the three stacks, in elements.  */
      YYSIZE_T yysize = yyssp - yyss + 1;

#ifdef yyoverflow
      {
	/* Give user a chance to reallocate the stack.  Use copies of
	   these so that the &'s don't force the real ones into
	   memory.  */
	YYSTYPE *yyvs1 = yyvs;
	yytype_int16 *yyss1 = yyss;

	/* Each stack pointer address is followed by the size of the
	   data in use in that stack, in bytes.  This used to be a
	   conditional around just the two extra args, but that might
	   be undefined if yyoverflow is a macro.  */
	yyoverflow (YY_("memory exhausted"),
		    &yyss1, yysize * sizeof (*yyssp),
		    &yyvs1, yysize * sizeof (*yyvsp),
		    &yystacksize);

	yyss = yyss1;
	yyvs = yyvs1;
      }
#else /* no yyoverflow */
# ifndef YYSTACK_RELOCATE
      goto yyexhaustedlab;
# else
      /* Extend the stack our own way.  */
      if (YYMAXDEPTH <= yystacksize)
	goto yyexhaustedlab;
      yystacksize *= 2;
      if (YYMAXDEPTH < yystacksize)
	yystacksize = YYMAXDEPTH;

      {
	yytype_int16 *yyss1 = yyss;
	union yyalloc *yyptr =
	  (union yyalloc *) YYSTACK_ALLOC (YYSTACK_BYTES (yystacksize));
	if (! yyptr)
	  goto yyexhaustedlab;
	YYSTACK_RELOCATE (yyss_alloc, yyss);
	YYSTACK_RELOCATE (yyvs_alloc, yyvs);
#  undef YYSTACK_RELOCATE
	if (yyss1 != yyssa)
	  YYSTACK_FREE (yyss1);
      }
# endif
#endif /* no yyoverflow */

      yyssp = yyss + yysize - 1;
      yyvsp = yyvs + yysize - 1;

      YYDPRINTF ((stderr, "Stack size increased to %lu\n",
		  (unsigned long int) yystacksize));

      if (yyss + yystacksize - 1 <= yyssp)
	YYABORT;
    }

  YYDPRINTF ((stderr, "Entering state %d\n", yystate));

  if (yystate == YYFINAL)
    YYACCEPT;

  goto yybackup;

/*-----------.
| yybackup.  |
`-----------*/
yybackup:

  /* Do appropriate processing given the current state.  Read a
     lookahead token if we need one and don't already have one.  */

  /* First try to decide what to do without reference to lookahead token.  */
  yyn = yypact[yystate];
  if (yyn == YYPACT_NINF)
    goto yydefault;

  /* Not known => get a lookahead token if don't already have one.  */

  /* YYCHAR is either YYEMPTY or YYEOF or a valid lookahead symbol.  */
  if (yychar == YYEMPTY)
    {
      YYDPRINTF ((stderr, "Reading a token: "));
      yychar = YYLEX;
    }

  if (yychar <= YYEOF)
    {
      yychar = yytoken = YYEOF;
      YYDPRINTF ((stderr, "Now at end of input.\n"));
    }
  else
    {
      yytoken = YYTRANSLATE (yychar);
      YY_SYMBOL_PRINT ("Next token is", yytoken, &yylval, &yylloc);
    }

  /* If the proper action on seeing token YYTOKEN is to reduce or to
     detect an error, take that action.  */
  yyn += yytoken;
  if (yyn < 0 || YYLAST < yyn || yycheck[yyn] != yytoken)
    goto yydefault;
  yyn = yytable[yyn];
  if (yyn <= 0)
    {
      if (yyn == 0 || yyn == YYTABLE_NINF)
	goto yyerrlab;
      yyn = -yyn;
      goto yyreduce;
    }

  /* Count tokens shifted since error; after three, turn off error
     status.  */
  if (yyerrstatus)
    yyerrstatus--;

  /* Shift the lookahead token.  */
  YY_SYMBOL_PRINT ("Shifting", yytoken, &yylval, &yylloc);

  /* Discard the shifted token.  */
  yychar = YYEMPTY;

  yystate = yyn;
  *++yyvsp = yylval;

  goto yynewstate;


/*-----------------------------------------------------------.
| yydefault -- do the default action for the current state.  |
`-----------------------------------------------------------*/
yydefault:
  yyn = yydefact[yystate];
  if (yyn == 0)
    goto yyerrlab;
  goto yyreduce;


/*-----------------------------.
| yyreduce -- Do a reduction.  |
`-----------------------------*/
yyreduce:
  /* yyn is the number of a rule to reduce with.  */
  yylen = yyr2[yyn];

  /* If YYLEN is nonzero, implement the default value of the action:
     `$$ = $1'.

     Otherwise, the following line sets YYVAL to garbage.
     This behavior is undocumented and Bison
     users should not rely upon it.  Assigning to YYVAL
     unconditionally makes the parser a bit smaller, and it avoids a
     GCC warning that YYVAL may be used uninitialized.  */
  yyval = yyvsp[1-yylen];


  YY_REDUCE_PRINT (yyn);
  switch (yyn)
    {
        case 2:

/* Line 1455 of yacc.c  */
#line 102 "spl.y"
    {
				ternaryTree parseTree = create_node((yyvsp[(1) - (6)].iVal), PROGRAM, create_node((yyvsp[(5) - (6)].iVal), PROGRAM, NULL, NULL, NULL), (yyvsp[(3) - (6)].tVal), NULL);
				
				for(int i = 0; i < currentSymbolTableSize; i++)
				{
					symbolTable[i] -> type = 'N';
					symbolTable[i] -> flag = 0;
				}
				
				#ifdef DEBUG
					printTree(parseTree, 0);
					
					printf("\n");
				#else
					printCode(parseTree);
				#endif
			;}
    break;

  case 3:

/* Line 1455 of yacc.c  */
#line 122 "spl.y"
    {
				(yyval.tVal) = create_node(NOTHING, BLOCK, (yyvsp[(2) - (2)].tVal), NULL, NULL);
			;}
    break;

  case 4:

/* Line 1455 of yacc.c  */
#line 126 "spl.y"
    {
				(yyval.tVal) = create_node(NOTHING, BLOCK, (yyvsp[(2) - (4)].tVal), (yyvsp[(4) - (4)].tVal), NULL);
			;}
    break;

  case 5:

/* Line 1455 of yacc.c  */
#line 132 "spl.y"
    {
							(yyval.tVal) = create_node(NOTHING, DECLARATION_LIST, (yyvsp[(1) - (1)].tVal), NULL, NULL);
						;}
    break;

  case 6:

/* Line 1455 of yacc.c  */
#line 136 "spl.y"
    {
							(yyval.tVal) = create_node(NOTHING, DECLARATION_LIST, (yyvsp[(1) - (2)].tVal), (yyvsp[(2) - (2)].tVal), NULL);
						;}
    break;

  case 7:

/* Line 1455 of yacc.c  */
#line 142 "spl.y"
    {
					(yyval.tVal) = create_node(NOTHING, DECLARATION, (yyvsp[(1) - (5)].tVal), (yyvsp[(4) - (5)].tVal), NULL);
				;}
    break;

  case 8:

/* Line 1455 of yacc.c  */
#line 148 "spl.y"
    {
				(yyval.tVal) = create_node(REAL, TYPE_Y, NULL, NULL, NULL);
			;}
    break;

  case 9:

/* Line 1455 of yacc.c  */
#line 152 "spl.y"
    {
				(yyval.tVal) = create_node(INTEGER, TYPE_Y, NULL, NULL, NULL);
			;}
    break;

  case 10:

/* Line 1455 of yacc.c  */
#line 156 "spl.y"
    {
				(yyval.tVal) = create_node(CHARACTER, TYPE_Y, NULL, NULL, NULL);
			;}
    break;

  case 11:

/* Line 1455 of yacc.c  */
#line 162 "spl.y"
    {
						(yyval.tVal) = create_node(NOTHING, STATEMENT_LIST, (yyvsp[(1) - (1)].tVal), NULL, NULL);
					;}
    break;

  case 12:

/* Line 1455 of yacc.c  */
#line 166 "spl.y"
    {
						(yyval.tVal) = create_node(NOTHING, STATEMENT_LIST, (yyvsp[(1) - (3)].tVal), (yyvsp[(3) - (3)].tVal), NULL);
					;}
    break;

  case 13:

/* Line 1455 of yacc.c  */
#line 172 "spl.y"
    {
					(yyval.tVal) = create_node(ASSIGNMENT_STATEMENT, STATEMENT, (yyvsp[(1) - (1)].tVal), NULL, NULL);
				;}
    break;

  case 14:

/* Line 1455 of yacc.c  */
#line 176 "spl.y"
    {
					(yyval.tVal) = create_node(IF_STATEMENT, STATEMENT, (yyvsp[(1) - (1)].tVal), NULL, NULL);
				;}
    break;

  case 15:

/* Line 1455 of yacc.c  */
#line 180 "spl.y"
    {
					(yyval.tVal) = create_node(DO_STATEMENT, STATEMENT, (yyvsp[(1) - (1)].tVal), NULL, NULL);
				;}
    break;

  case 16:

/* Line 1455 of yacc.c  */
#line 184 "spl.y"
    {
					(yyval.tVal) = create_node(WHILE_STATEMENT, STATEMENT, (yyvsp[(1) - (1)].tVal), NULL, NULL);
				;}
    break;

  case 17:

/* Line 1455 of yacc.c  */
#line 188 "spl.y"
    {
					(yyval.tVal) = create_node(FOR_STATEMENT, STATEMENT, (yyvsp[(1) - (1)].tVal), NULL, NULL);
				;}
    break;

  case 18:

/* Line 1455 of yacc.c  */
#line 192 "spl.y"
    {
					(yyval.tVal) = create_node(WRITE_STATEMENT, STATEMENT, (yyvsp[(1) - (1)].tVal), NULL, NULL);
				;}
    break;

  case 19:

/* Line 1455 of yacc.c  */
#line 196 "spl.y"
    {
					(yyval.tVal) = create_node(READ_STATEMENT, STATEMENT, (yyvsp[(1) - (1)].tVal), NULL, NULL);
				;}
    break;

  case 20:

/* Line 1455 of yacc.c  */
#line 202 "spl.y"
    {
								(yyval.tVal) = create_node((yyvsp[(3) - (3)].iVal), ASSIGNMENT_STATEMENT, (yyvsp[(1) - (3)].tVal), NULL, NULL);
							;}
    break;

  case 21:

/* Line 1455 of yacc.c  */
#line 208 "spl.y"
    {
						(yyval.tVal) = create_node(NOTHING, IF_STATEMENT, (yyvsp[(2) - (5)].tVal), (yyvsp[(4) - (5)].tVal), NULL);
					;}
    break;

  case 22:

/* Line 1455 of yacc.c  */
#line 212 "spl.y"
    {
						(yyval.tVal) = create_node(NOTHING, IF_STATEMENT, (yyvsp[(2) - (7)].tVal), (yyvsp[(4) - (7)].tVal), (yyvsp[(6) - (7)].tVal));
					;}
    break;

  case 23:

/* Line 1455 of yacc.c  */
#line 218 "spl.y"
    {
						(yyval.tVal) = create_node(NOTHING, DO_STATEMENT, (yyvsp[(2) - (5)].tVal), (yyvsp[(4) - (5)].tVal), NULL);
					;}
    break;

  case 24:

/* Line 1455 of yacc.c  */
#line 224 "spl.y"
    {
						(yyval.tVal) = create_node(NOTHING, WHILE_STATEMENT, (yyvsp[(2) - (5)].tVal), (yyvsp[(4) - (5)].tVal), NULL);
					;}
    break;

  case 25:

/* Line 1455 of yacc.c  */
#line 230 "spl.y"
    {
						(yyval.tVal) = create_node((yyvsp[(2) - (11)].iVal), FOR_STATEMENT, create_node(NOTHING, FOR_STATEMENT, (yyvsp[(4) - (11)].tVal), (yyvsp[(6) - (11)].tVal), (yyvsp[(8) - (11)].tVal)), (yyvsp[(10) - (11)].tVal), NULL);
					;}
    break;

  case 26:

/* Line 1455 of yacc.c  */
#line 236 "spl.y"
    {
						(yyval.tVal) = create_node(NOTHING, WRITE_STATEMENT, NULL, NULL, NULL);
					;}
    break;

  case 27:

/* Line 1455 of yacc.c  */
#line 240 "spl.y"
    {
						(yyval.tVal) = create_node(NOTHING, WRITE_STATEMENT, (yyvsp[(3) - (4)].tVal), NULL, NULL);
					;}
    break;

  case 28:

/* Line 1455 of yacc.c  */
#line 246 "spl.y"
    {
						(yyval.tVal) = create_node((yyvsp[(3) - (4)].iVal), READ_STATEMENT, NULL, NULL, NULL);
					;}
    break;

  case 29:

/* Line 1455 of yacc.c  */
#line 252 "spl.y"
    {
					(yyval.tVal) = create_node(NOTHING, OUTPUT_LIST, (yyvsp[(1) - (1)].tVal), NULL, NULL);
				;}
    break;

  case 30:

/* Line 1455 of yacc.c  */
#line 256 "spl.y"
    {
					(yyval.tVal) = create_node(NOTHING, OUTPUT_LIST, (yyvsp[(1) - (3)].tVal), (yyvsp[(3) - (3)].tVal), NULL);
				;}
    break;

  case 31:

/* Line 1455 of yacc.c  */
#line 262 "spl.y"
    {
							(yyval.tVal) = create_node(NOTHING, CONDITIONAL_LIST, (yyvsp[(1) - (1)].tVal), NULL, NULL);
						;}
    break;

  case 32:

/* Line 1455 of yacc.c  */
#line 266 "spl.y"
    {
							(yyval.tVal) = create_node(OR, CONDITIONAL_LIST, (yyvsp[(1) - (3)].tVal), (yyvsp[(3) - (3)].tVal), NULL);
						;}
    break;

  case 33:

/* Line 1455 of yacc.c  */
#line 270 "spl.y"
    {
							(yyval.tVal) = create_node(AND, CONDITIONAL_LIST, (yyvsp[(1) - (3)].tVal), (yyvsp[(3) - (3)].tVal), NULL);
						;}
    break;

  case 34:

/* Line 1455 of yacc.c  */
#line 276 "spl.y"
    {
					(yyval.tVal) = create_node(NOTHING, CONDITIONAL, (yyvsp[(2) - (2)].tVal), NULL, NULL);
				;}
    break;

  case 35:

/* Line 1455 of yacc.c  */
#line 280 "spl.y"
    {
					(yyval.tVal) = create_node(NOTHING, CONDITIONAL, (yyvsp[(1) - (3)].tVal), (yyvsp[(2) - (3)].tVal), (yyvsp[(3) - (3)].tVal));
				;}
    break;

  case 36:

/* Line 1455 of yacc.c  */
#line 285 "spl.y"
    {
					(yyval.tVal) = create_node(EQUAL, COMPARATOR, NULL, NULL, NULL);
				;}
    break;

  case 37:

/* Line 1455 of yacc.c  */
#line 289 "spl.y"
    {
					(yyval.tVal) = create_node(LESS_THAN, COMPARATOR, NULL, NULL, NULL);
				;}
    break;

  case 38:

/* Line 1455 of yacc.c  */
#line 293 "spl.y"
    {
					(yyval.tVal) = create_node(GREATER_THAN, COMPARATOR, NULL, NULL, NULL);
				;}
    break;

  case 39:

/* Line 1455 of yacc.c  */
#line 297 "spl.y"
    {
					(yyval.tVal) = create_node(LESS_THAN_EQUAL, COMPARATOR, NULL, NULL, NULL);
				;}
    break;

  case 40:

/* Line 1455 of yacc.c  */
#line 301 "spl.y"
    {
					(yyval.tVal) = create_node(GREATER_THAN_EQUAL, COMPARATOR, NULL, NULL, NULL);
				;}
    break;

  case 41:

/* Line 1455 of yacc.c  */
#line 305 "spl.y"
    {
					(yyval.tVal) = create_node(LESS_THAN_GREATER_THAN, COMPARATOR, NULL, NULL, NULL);
				;}
    break;

  case 42:

/* Line 1455 of yacc.c  */
#line 311 "spl.y"
    {
					(yyval.tVal) = create_node(NOTHING, EXPRESSION, (yyvsp[(1) - (1)].tVal), NULL, NULL);
				;}
    break;

  case 43:

/* Line 1455 of yacc.c  */
#line 315 "spl.y"
    {
					(yyval.tVal) = create_node(PLUS, EXPRESSION, (yyvsp[(1) - (3)].tVal), (yyvsp[(3) - (3)].tVal), NULL);
				;}
    break;

  case 44:

/* Line 1455 of yacc.c  */
#line 319 "spl.y"
    {
					(yyval.tVal) = create_node(HYPHEN, EXPRESSION, (yyvsp[(1) - (3)].tVal), (yyvsp[(3) - (3)].tVal), NULL);
				;}
    break;

  case 45:

/* Line 1455 of yacc.c  */
#line 325 "spl.y"
    {
				(yyval.tVal) = create_node(NOTHING, TERM, (yyvsp[(1) - (1)].tVal), NULL, NULL);
			;}
    break;

  case 46:

/* Line 1455 of yacc.c  */
#line 329 "spl.y"
    {
				(yyval.tVal) = create_node(ASTERIX, TERM, (yyvsp[(1) - (3)].tVal), (yyvsp[(3) - (3)].tVal), NULL);
			;}
    break;

  case 47:

/* Line 1455 of yacc.c  */
#line 333 "spl.y"
    {
				(yyval.tVal) = create_node(FORWARD_SLASH, TERM, (yyvsp[(1) - (3)].tVal), (yyvsp[(3) - (3)].tVal), NULL);
			;}
    break;

  case 48:

/* Line 1455 of yacc.c  */
#line 339 "spl.y"
    {
				(yyval.tVal) = create_node(CONSTANT, VALUE, (yyvsp[(1) - (1)].tVal), NULL, NULL);
			;}
    break;

  case 49:

/* Line 1455 of yacc.c  */
#line 343 "spl.y"
    {
				(yyval.tVal) = create_node((yyvsp[(1) - (1)].iVal), VALUE, NULL, NULL, NULL);
			;}
    break;

  case 50:

/* Line 1455 of yacc.c  */
#line 347 "spl.y"
    {
				(yyval.tVal) = create_node(EXPRESSION, VALUE, (yyvsp[(2) - (3)].tVal), NULL, NULL);
			;}
    break;

  case 51:

/* Line 1455 of yacc.c  */
#line 353 "spl.y"
    {
					(yyval.tVal) = create_node(NOTHING, CONSTANT, (yyvsp[(1) - (1)].tVal), NULL, NULL);
				;}
    break;

  case 52:

/* Line 1455 of yacc.c  */
#line 357 "spl.y"
    {
					(yyval.tVal) = create_node((yyvsp[(1) - (1)].iVal), CONSTANT, NULL, NULL, NULL);
				;}
    break;

  case 53:

/* Line 1455 of yacc.c  */
#line 363 "spl.y"
    {
						(yyval.tVal) = create_node((yyvsp[(1) - (1)].iVal), NUMBER_CONSTANT, NULL, NULL, NULL);
					;}
    break;

  case 54:

/* Line 1455 of yacc.c  */
#line 367 "spl.y"
    {
						(yyval.tVal) = create_node((yyvsp[(2) - (2)].iVal), NEGATIVE_NUMBER_CONSTANT, NULL, NULL, NULL);
					;}
    break;

  case 55:

/* Line 1455 of yacc.c  */
#line 371 "spl.y"
    {
						(yyval.tVal) = create_node((yyvsp[(1) - (1)].iVal), FLOATING_NUMBER_CONSTANT, NULL, NULL, NULL);
					;}
    break;

  case 56:

/* Line 1455 of yacc.c  */
#line 375 "spl.y"
    {
						(yyval.tVal) = create_node((yyvsp[(2) - (2)].iVal), FLOATING_NEGATIVE_NUMBER_CONSTANT, NULL, NULL, NULL);
					;}
    break;

  case 57:

/* Line 1455 of yacc.c  */
#line 381 "spl.y"
    {
						(yyval.tVal) = create_node((yyvsp[(1) - (1)].iVal), IDENTIFIER_LIST, NULL, NULL, NULL);
					;}
    break;

  case 58:

/* Line 1455 of yacc.c  */
#line 385 "spl.y"
    {
						(yyval.tVal) = create_node((yyvsp[(3) - (3)].iVal), IDENTIFIER_LIST, (yyvsp[(1) - (3)].tVal), NULL, NULL);
					;}
    break;



/* Line 1455 of yacc.c  */
#line 2086 "spl.tab.c"
      default: break;
    }
  YY_SYMBOL_PRINT ("-> $$ =", yyr1[yyn], &yyval, &yyloc);

  YYPOPSTACK (yylen);
  yylen = 0;
  YY_STACK_PRINT (yyss, yyssp);

  *++yyvsp = yyval;

  /* Now `shift' the result of the reduction.  Determine what state
     that goes to, based on the state we popped back to and the rule
     number reduced by.  */

  yyn = yyr1[yyn];

  yystate = yypgoto[yyn - YYNTOKENS] + *yyssp;
  if (0 <= yystate && yystate <= YYLAST && yycheck[yystate] == *yyssp)
    yystate = yytable[yystate];
  else
    yystate = yydefgoto[yyn - YYNTOKENS];

  goto yynewstate;


/*------------------------------------.
| yyerrlab -- here on detecting error |
`------------------------------------*/
yyerrlab:
  /* If not already recovering from an error, report this error.  */
  if (!yyerrstatus)
    {
      ++yynerrs;
#if ! YYERROR_VERBOSE
      yyerror (YY_("syntax error"));
#else
      {
	YYSIZE_T yysize = yysyntax_error (0, yystate, yychar);
	if (yymsg_alloc < yysize && yymsg_alloc < YYSTACK_ALLOC_MAXIMUM)
	  {
	    YYSIZE_T yyalloc = 2 * yysize;
	    if (! (yysize <= yyalloc && yyalloc <= YYSTACK_ALLOC_MAXIMUM))
	      yyalloc = YYSTACK_ALLOC_MAXIMUM;
	    if (yymsg != yymsgbuf)
	      YYSTACK_FREE (yymsg);
	    yymsg = (char *) YYSTACK_ALLOC (yyalloc);
	    if (yymsg)
	      yymsg_alloc = yyalloc;
	    else
	      {
		yymsg = yymsgbuf;
		yymsg_alloc = sizeof yymsgbuf;
	      }
	  }

	if (0 < yysize && yysize <= yymsg_alloc)
	  {
	    (void) yysyntax_error (yymsg, yystate, yychar);
	    yyerror (yymsg);
	  }
	else
	  {
	    yyerror (YY_("syntax error"));
	    if (yysize != 0)
	      goto yyexhaustedlab;
	  }
      }
#endif
    }



  if (yyerrstatus == 3)
    {
      /* If just tried and failed to reuse lookahead token after an
	 error, discard it.  */

      if (yychar <= YYEOF)
	{
	  /* Return failure if at end of input.  */
	  if (yychar == YYEOF)
	    YYABORT;
	}
      else
	{
	  yydestruct ("Error: discarding",
		      yytoken, &yylval);
	  yychar = YYEMPTY;
	}
    }

  /* Else will try to reuse lookahead token after shifting the error
     token.  */
  goto yyerrlab1;


/*---------------------------------------------------.
| yyerrorlab -- error raised explicitly by YYERROR.  |
`---------------------------------------------------*/
yyerrorlab:

  /* Pacify compilers like GCC when the user code never invokes
     YYERROR and the label yyerrorlab therefore never appears in user
     code.  */
  if (/*CONSTCOND*/ 0)
     goto yyerrorlab;

  /* Do not reclaim the symbols of the rule which action triggered
     this YYERROR.  */
  YYPOPSTACK (yylen);
  yylen = 0;
  YY_STACK_PRINT (yyss, yyssp);
  yystate = *yyssp;
  goto yyerrlab1;


/*-------------------------------------------------------------.
| yyerrlab1 -- common code for both syntax error and YYERROR.  |
`-------------------------------------------------------------*/
yyerrlab1:
  yyerrstatus = 3;	/* Each real token shifted decrements this.  */

  for (;;)
    {
      yyn = yypact[yystate];
      if (yyn != YYPACT_NINF)
	{
	  yyn += YYTERROR;
	  if (0 <= yyn && yyn <= YYLAST && yycheck[yyn] == YYTERROR)
	    {
	      yyn = yytable[yyn];
	      if (0 < yyn)
		break;
	    }
	}

      /* Pop the current state because it cannot handle the error token.  */
      if (yyssp == yyss)
	YYABORT;


      yydestruct ("Error: popping",
		  yystos[yystate], yyvsp);
      YYPOPSTACK (1);
      yystate = *yyssp;
      YY_STACK_PRINT (yyss, yyssp);
    }

  *++yyvsp = yylval;


  /* Shift the error token.  */
  YY_SYMBOL_PRINT ("Shifting", yystos[yyn], yyvsp, yylsp);

  yystate = yyn;
  goto yynewstate;


/*-------------------------------------.
| yyacceptlab -- YYACCEPT comes here.  |
`-------------------------------------*/
yyacceptlab:
  yyresult = 0;
  goto yyreturn;

/*-----------------------------------.
| yyabortlab -- YYABORT comes here.  |
`-----------------------------------*/
yyabortlab:
  yyresult = 1;
  goto yyreturn;

#if !defined(yyoverflow) || YYERROR_VERBOSE
/*-------------------------------------------------.
| yyexhaustedlab -- memory exhaustion comes here.  |
`-------------------------------------------------*/
yyexhaustedlab:
  yyerror (YY_("memory exhausted"));
  yyresult = 2;
  /* Fall through.  */
#endif

yyreturn:
  if (yychar != YYEMPTY)
     yydestruct ("Cleanup: discarding lookahead",
		 yytoken, &yylval);
  /* Do not reclaim the symbols of the rule which action triggered
     this YYABORT or YYACCEPT.  */
  YYPOPSTACK (yylen);
  YY_STACK_PRINT (yyss, yyssp);
  while (yyssp != yyss)
    {
      yydestruct ("Cleanup: popping",
		  yystos[*yyssp], yyvsp);
      YYPOPSTACK (1);
    }
#ifndef yyoverflow
  if (yyss != yyssa)
    YYSTACK_FREE (yyss);
#endif
#if YYERROR_VERBOSE
  if (yymsg != yymsgbuf)
    YYSTACK_FREE (yymsg);
#endif
  /* Make sure YYID is used.  */
  return YYID (yyresult);
}



/* Line 1675 of yacc.c  */
#line 390 "spl.y"


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
			yyerror("unknown identifier");
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
				yyerror("unknown nodeIdentifier");
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
						if(t -> item != CONSTANT && t -> item != EXPRESSION)
						{
							printIdentifier(t);
						}
						else
						{
							printf("\n");
						}
						
						break;
					
					case CONSTANT:
						printf(" Character: %c\n", t -> item);
						
						break;
						
					case NUMBER_CONSTANT:
						printf(" Number: %d\n", t -> item);
						
						break;
					
					case NEGATIVE_NUMBER_CONSTANT:
						printf(" Number: -%d\n", t -> item);
						
						break;
					
					case FLOATING_NUMBER_CONSTANT:
						printf(" Number: %s\n", symbolTable[t -> item] -> identifier);
						
						break;
					
					case FLOATING_NEGATIVE_NUMBER_CONSTANT:
						printf(" Number: -%s\n", symbolTable[t -> item] -> identifier);
						
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
					
				case VALUE:
					if(t -> first == NULL)
					{
						if(t -> item < 0 || t -> item > currentSymbolTableSize)
						{
							yyerror("unknown type");
						}
						else
						{
							if(symbolTable[t -> item] -> type == 'N')
							{
								yyerror("unknown type");
							}
							else
							{
								constantFoldingFlag = 0;
								
								switch(symbolTable[t -> item] -> type)
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
					}
					
					break;
			}
			
			expressionType(t -> first);
			expressionType(t -> second);
			expressionType(t -> third);
		}
	}
	
	void floatConstantFolding(ternaryTree t)
	{
		if(t != NULL)
		{
			switch(t -> nodeIdentifier)
			{
				case CONSTANT:
					if(t -> item != NOTHING)
					{
						if(leftRightFlag == 0)
						{
							leftRightFlag = 1;
							
							floatLeft = t -> item;
						}
						else
						{
							floatRight = t -> item;
							
							breakFlag = 1;
						}
					}
					else
					{
						if(breakFlag == 0)
						{
							floatConstantFolding(t -> first);
							floatConstantFolding(t -> second);
							floatConstantFolding(t -> third);
						}
					}
					
					break;
					
				case NUMBER_CONSTANT:
					if(leftRightFlag == 0)
					{
						leftRightFlag = 1;
						
						floatLeft = t -> item;
					}
					else
					{
						floatRight = t -> item;
						
						breakFlag = 1;
					}
					
					break;
				
				case NEGATIVE_NUMBER_CONSTANT:
					if(leftRightFlag == 0)
					{
						leftRightFlag = 1;
						
						floatLeft = - (t -> item);
					}
					else
					{
						floatRight = - (t -> item);
						
						breakFlag = 1;
					}
					
					break;
				
				case FLOATING_NUMBER_CONSTANT:
					if(leftRightFlag == 0)
					{
						leftRightFlag = 1;
						
						floatLeft = atof(symbolTable[t -> item] -> identifier);
					}
					else
					{
						floatRight = atof(symbolTable[t -> item] -> identifier);
						
						breakFlag = 1;
					}
					
					break;
				
				case FLOATING_NEGATIVE_NUMBER_CONSTANT:
					if(leftRightFlag == 0)
					{
						leftRightFlag = 1;
						
						floatLeft = - (atof(symbolTable[t -> item] -> identifier));
					}
					else
					{
						floatRight = - (atof(symbolTable[t -> item] -> identifier));
						
						breakFlag = 1;
					}
					
					break;
				
				default:
					if(breakFlag == 0)
					{
						floatConstantFolding(t -> first);
						floatConstantFolding(t -> second);
						floatConstantFolding(t -> third);
					}
					
					break;
			}
		}
	}
	
	void intConstantFolding(ternaryTree t)
	{
		printf(" %d %d ", intLeft, intRight);
		if(t != NULL)
		{
			switch(t -> nodeIdentifier)
			{
				case CONSTANT:
					if(t -> item != NOTHING)
					{
						if(leftRightFlag == 0)
						{
							leftRightFlag = 1;
							
							intLeft = t -> item;
						}
						else
						{
							intRight = t -> item;
							
							breakFlag = 1;
						}
					}
					else
					{
						if(breakFlag == 0)
						{
							intConstantFolding(t -> first);
							intConstantFolding(t -> second);
							intConstantFolding(t -> third);
						}
					}
					
					break;
					
				case NUMBER_CONSTANT:
					if(leftRightFlag == 0)
					{
						leftRightFlag = 1;
						
						intLeft = t -> item;
					}
					else
					{
						intRight = t -> item;
						
						breakFlag = 1;
					}
					
					break;
				
				case NEGATIVE_NUMBER_CONSTANT:
					if(leftRightFlag == 0)
					{
						leftRightFlag = 1;
						
						intLeft = - (t -> item);
					}
					else
					{
						intRight = - (t -> item);
						
						breakFlag = 1;
					}
					
					break;
				
				case FLOATING_NUMBER_CONSTANT:
					if(leftRightFlag == 0)
					{
						leftRightFlag = 1;
						
						intLeft = atoi(symbolTable[t -> item] -> identifier);
					}
					else
					{
						intRight = atoi(symbolTable[t -> item] -> identifier);
						
						breakFlag = 1;
					}
					
					break;
				
				case FLOATING_NEGATIVE_NUMBER_CONSTANT:
					if(leftRightFlag == 0)
					{
						leftRightFlag = 1;
						
						intLeft = - (atoi(symbolTable[t -> item] -> identifier));
					}
					else
					{
						intRight = - (atoi(symbolTable[t -> item] -> identifier));
						
						breakFlag = 1;
					}
					
					break;
				
				default:
					if(breakFlag == 0)
					{
						intConstantFolding(t -> first);
						intConstantFolding(t -> second);
						intConstantFolding(t -> third);
					}
					
					break;
			}
		}
	}
	
	void printCode(ternaryTree t)
	{
		if(t != NULL)
		{	
			switch(t -> nodeIdentifier)
			{
				case PROGRAM:
					if(!((t -> item < 0 || t -> item > currentSymbolTableSize) && (t -> first -> item < 0 || t -> first -> item > currentSymbolTableSize)))
					{
						if(symbolTable[t -> item] -> identifier == symbolTable[t -> first -> item] -> identifier)
						{
							printf("#include <stdio.h>\nint main(void)\n{\n");
					
							printCode(t -> second);
					
							printf("return 0;\n}\n");
						}
						else
						{
							yyerror("unknown identifier");
						}
					}
					else
					{
						yyerror("unknown identifier");
					}
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
						yyerror("unknown identifier");
					}
					else
					{
						symbolTable[t -> item] -> flag = 1;
						
						printf("%s = ", symbolTable[t -> item] -> identifier);
					}
					
					if(t -> first -> first -> first -> item == CONSTANT)
					{
						if(t -> first -> first -> first -> first -> item != NOTHING)
						{
							printf("'");
							
							printCode(t -> first);
							
							printf("'");
						}
						else
						{
							if(symbolTable[t -> item] -> type == 'c')
							{
								yyerror("invalid character");
							}
							else
							{
								printCode(t -> first);
							}
						}
					}
					else
					{
						if(t -> first -> first -> first -> item != EXPRESSION)
						{
							if(symbolTable[t -> first -> first -> first -> item] -> flag == 0)
							{
								yyerror("unassigned identifier");
							}
							else
							{
								if(symbolTable[t -> item] -> type == 'c' && (symbolTable[t -> first -> first -> first -> item] -> type == 'd' || symbolTable[t -> first -> first -> first -> item] -> type == 'f'))
								{
									yyerror("invalid character");
								}
								else
								{
									printCode(t -> first);
								}
							}
						}
						else
						{
							printCode(t -> first);
						}
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
					forLoopInt++;
					
					if(t -> item < 0 || t -> item > currentSymbolTableSize)
					{
						yyerror("unknown identifier");
					}
					else
					{
						symbolTable[t -> item] -> flag = 1;
						
						if(t -> first -> second -> first -> first -> item == CONSTANT)
						{
							if(t -> first -> second -> first -> first -> first -> item != NOTHING)
							{
								printf("register char _by%d;\nfor(%s = ", forLoopInt, symbolTable[t -> item] -> identifier);
							}
							else
							{
								if(t -> first -> second -> first -> first -> first -> first -> nodeIdentifier == FLOATING_NUMBER_CONSTANT 
								|| t -> first -> second -> first -> first -> first -> first -> nodeIdentifier == FLOATING_NEGATIVE_NUMBER_CONSTANT)
								{
									printf("register float _by%d;\nfor(%s = ", forLoopInt, symbolTable[t -> item] -> identifier);
								}
								else
								{
									printf("register int _by%d;\nfor(%s = ", forLoopInt, symbolTable[t -> item] -> identifier);
								}
							}
						}
						else
						{
							if(t -> first -> second -> first -> first -> first == NULL)
							{
								switch(symbolTable[t -> first -> second -> first -> first -> item] -> type)
								{
									case 'f':
										printf("register float _by%d;\nfor(%s = ", forLoopInt, symbolTable[t -> item] -> identifier);
										
										break;
									
									case 'd':
										printf("register int _by%d;\nfor(%s = ", forLoopInt, symbolTable[t -> item] -> identifier);
										
										break;
									
									case 'c':
										printf("register char _by%d;\nfor(%s = ", forLoopInt, symbolTable[t -> item] -> identifier);
										
										break;
								}
							}
						}
					}
					
					if(t -> first -> first -> first -> first -> item == CONSTANT && t -> first -> first -> first -> first -> first -> item != NOTHING)
					{
						printf("'");
						
						printCode(t -> first -> first);
						
						printf("'");
					}
					else
					{
						printCode(t -> first -> first);
					}
					
					printf("; _by%d = ", forLoopInt);
					
					if(t -> first -> second -> first -> first -> item == CONSTANT && t -> first -> second -> first -> first -> first -> item != NOTHING)
					{
						printf("'");
						
						printCode(t -> first -> second);
						
						printf("'");
					}
					else
					{
						printCode(t -> first -> second);
					}
					
					if(t -> item < 0 || t -> item > currentSymbolTableSize)
					{
						yyerror("unknown identifier");
					}
					else
					{
						printf(", (%s - ", symbolTable[t -> item] -> identifier);
					}
					
					if(t -> first -> third -> first -> first -> item == CONSTANT && t -> first -> third -> first -> first -> first -> item != NOTHING)
					{
						printf("'");
						
						printCode(t -> first -> third);
						
						printf("'");
					}
					else
					{
						printCode(t -> first -> third);
					}
					
					if(t -> item < 0 || t -> item > currentSymbolTableSize)
					{
						yyerror("unknown identifier");
					}
					else
					{
						printf(") * ((_by%d > 0) - (_by%d < 0)) <= 0; %s += _by%d)\n{\n", forLoopInt, forLoopInt, symbolTable[t -> item] -> identifier, forLoopInt);
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
						yyerror("unknown type");
					}
					else
					{
						if(symbolTable[t -> item] -> type == 'N')
						{
							yyerror("unknown type");
						}
						else
						{
							printf("scanf(\" %%%c\", &%s);\n", symbolTable[t -> item] -> type, symbolTable[t -> item] -> identifier);
						}
						
						symbolTable[t -> item] -> flag = 1;
					}
					
					break;
				
				case OUTPUT_LIST:
					printf("printf(\"");
					
					if(t -> first -> item == EXPRESSION)
					{
						expressionCharacterType = 'N';
						expressionType(t -> first -> first);
						
						if(expressionCharacterType == 'N')
						{
							yyerror("unknown type");
						}
						else
						{
							printf("%%%c\", ", expressionCharacterType);
						}
						
						expressionCharacterType = 'N';
					}
					else
					{
						if(t -> first -> first == NULL)
						{
							
							if(t -> first -> item < 0 || t -> first -> item > currentSymbolTableSize)
							{
								yyerror("unknown type");
							}
							else
							{
								if(symbolTable[t -> first -> item] -> flag == 0)
								{
									yyerror("unassigned identifier");
								}
								else
								{
									if(symbolTable[t -> first -> item] -> type == 'N')
									{
										yyerror("unknown type");
									}
									else
									{
										printf("%%%c\", ", symbolTable[t -> first -> item] -> type);
									}
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
						constantFoldingFlag = 1;
						expressionCharacterType = 'N';
						expressionType(t);
						
						#ifndef OPTIMISATION
							constantFoldingFlag = -1;
						#endif
						
						if(constantFoldingFlag == 1)
						{
							leftRightFlag = 0;
							
							switch(expressionCharacterType)
							{
								case 'f':
									breakFlag = 0;
									floatConstantFolding(t);
									breakFlag = 0;
									
									float floatExpressionResult;
									
									if(t -> item == PLUS)
									{
										floatExpressionResult = floatLeft + floatRight;
									}
									else
									{
										floatExpressionResult = floatLeft - floatRight;
									}
									
									printf("%f", floatExpressionResult);
									
									floatLeft = 0;
									floatRight = 0;
									leftRightFlag = 0;
									
									break;
								
								case 'd':									
									breakFlag = 0;
									intConstantFolding(t);
									breakFlag = 0;
									
									int intExpressionResult;
									
									if(t -> item == PLUS)
									{
										intExpressionResult = intLeft + intRight;
									}
									else
									{
										intExpressionResult = intLeft - intRight;
									}
									
									printf("%d", intExpressionResult);
									
									intLeft = 0;
									intRight = 0;
									leftRightFlag = 0;
									
									break;
								
								case 'c':									
									breakFlag = 0;
									intConstantFolding(t);
									breakFlag = 0;
									
									int charExpressionResultTest;
									char charExpressionResult;
									
									if(t -> item == PLUS)
									{
										charExpressionResultTest = intLeft + intRight;
									}
									else
									{
										charExpressionResultTest = intLeft - intRight;
									}
									
									if(charExpressionResultTest < 0 || charExpressionResultTest > 127)
									{
										yyerror("invalid character");
									}
									else
									{
										charExpressionResult = charExpressionResultTest;
									}
									
									printf("%c", charExpressionResult);
									
									intLeft = 0;
									intRight = 0;
									leftRightFlag = 0;
									
									break;
							}
							
							constantFoldingFlag = 1;
							expressionCharacterType = 'N';
						}
						else
						{
							printf("(");
							
							if(t -> first -> first -> first != NULL)
							{
								if(t -> first -> first -> first -> first == NULL)
								{
									printf("'");
									
									printCode(t -> first);
									
									printf("'");
								}
								else
								{
									printCode(t -> first);
								}
							}
							else
							{
								printCode(t -> first);
							}
							
							if(t -> item == PLUS)
							{
								printf(" + ");
							}
							else
							{
								printf(" - ");
							}
							
							if(t -> second -> first -> first -> first != NULL)
							{
								if(t -> second -> first -> first -> first -> first == NULL)
								{
									printf("'");
									
									printCode(t -> second);
									
									printf("'");
								}
								else
								{
									printCode(t -> second);
								}
							}
							else
							{
								printCode(t -> second);
							}
							
							printf(")");
						}
					}
					else
					{
						printCode(t -> first);
					}
					
					break;
				
				case TERM:
					if(t -> second != NULL)
					{
						constantFoldingFlag = 1;
						expressionCharacterType = 'N';
						expressionType(t);
						
						#ifndef OPTIMISATION
							constantFoldingFlag = -1;
						#endif
						
						if(constantFoldingFlag == 1)
						{
							switch(expressionCharacterType)
							{
								leftRightFlag = 0;
								
								case 'f':									
									breakFlag = 0;
									floatConstantFolding(t);
									breakFlag = 0;
									
									float floatExpressionResult;
									
									if(t -> item == ASTERIX)
									{
										floatExpressionResult = floatLeft * floatRight;
									}
									else
									{
										floatExpressionResult = floatLeft / floatRight;
									}
									
									printf("%f", floatExpressionResult);
									
									floatLeft = 0;
									floatRight = 0;
									leftRightFlag = 0;
									
									break;
								
								case 'd':
									breakFlag = 0;
									intConstantFolding(t);
									breakFlag = 0;
									
									int intExpressionResult;
									
									
									if(t -> item == ASTERIX)
									{
										intExpressionResult = intLeft * intRight;
									}
									else
									{
										intExpressionResult = intLeft / intRight;
									}
									
									printf("%d", intExpressionResult);
									
									intLeft = 0;
									intRight = 0;
									leftRightFlag = 0;
									
									break;
								
								case 'c':
									breakFlag = 0;
									intConstantFolding(t);
									breakFlag = 0;
									
									int charExpressionResultTest;
									char charExpressionResult;
									
									if(t -> item == PLUS)
									{
										charExpressionResultTest = intLeft * intRight;
									}
									else
									{
										charExpressionResultTest = intLeft / intRight;
									}
									
									if(charExpressionResultTest < 0 || charExpressionResultTest > 127)
									{
										yyerror("invalid character");
									}
									else
									{
										charExpressionResult = charExpressionResultTest;
									}
									
									printf("%c", charExpressionResult);
									
									intLeft = 0;
									intRight = 0;
									leftRightFlag = 0;
									
									break;
							}
							
							constantFoldingFlag = 1;
							expressionCharacterType = 'N';
						}
						else
						{
							printf("(");
						
							if(t -> first -> first != NULL)
							{
								if(t -> first -> first -> first == NULL)
								{
									printf("'");
									
									printCode(t -> first);
									
									printf("'");
								}
								else
								{
									printCode(t -> first);
								}
							}
							else
							{
								printCode(t -> first);
							}
							
							if(t -> item == ASTERIX)
							{
								printf(" * ");
							}
							else
							{
								printf(" / ");
							}
							
							if(t -> second -> first -> first != NULL)
							{
								if(t -> second -> first -> first -> first == NULL)
								{
									printf("'");
									
									printCode(t -> second);
									
									printf("'");
								}
								else
								{
									printCode(t -> second);
								}
							}
							else
							{
								printCode(t -> second);
							}
							
							printf(")");
						}
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
							yyerror("unknown identifier");
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
					printf("%s", symbolTable[t -> item] -> identifier);
					
					break;
				
				case FLOATING_NEGATIVE_NUMBER_CONSTANT:
					printf("-%s", symbolTable[t -> item] -> identifier);
					
					break;
				
				case IDENTIFIER_LIST:
					if(t -> item < 0 || t -> item > currentSymbolTableSize)
					{
						yyerror("unknown identifier");
					}
					else
					{
						if(symbolTable[t -> item] -> type == 'N')
						{							
							strcpy(underscore, constantUnderscore);
							strncat(underscore, symbolTable[t -> item] -> identifier, IDLENGTH);
							strcpy(symbolTable[t -> item] -> identifier, underscore);
							
							symbolTable[t -> item] -> type = identifierType;
							printf("%s", symbolTable[t -> item] -> identifier);
						}
						else
						{
							yyerror("duplicate identifier");
						}
					}
					
					if(t -> first != NULL)
					{
						printf(", ");
						
						printCode(t -> first);
					}
					
					break;
			}
		}
	}
#endif

#include "lex.yy.c"
