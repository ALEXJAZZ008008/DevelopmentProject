#include <stdio.h>
int main(void)
{
int  _a;
register int _by1;
for(_a = 1; _by1 = 1, (_a - 5) * ((_by1 > 0) - (_by1 < 0)) <= 0; _a += _by1)
{
printf("%d", _a);
}
printf("\n");
return 0;
}
