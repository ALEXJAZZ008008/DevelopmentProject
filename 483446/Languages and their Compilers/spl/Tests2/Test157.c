#include <stdio.h>
int main(void)
{
char  _a;
register int _by1;
for(_a = 'a'; _by1 = 1, (_a - 'g') * ((_by1 > 0) - (_by1 < 0)) <= 0; _a += _by1)
{
printf("%c", _a);
}
printf("\n");
return 0;
}
