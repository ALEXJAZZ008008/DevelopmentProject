#include <stdio.h>
int main(void)
{
int  _a;
register int _by1;
for(_a = 1; _by1 = 1, (_a - 13) * ((_by1 > 0) - (_by1 < 0)) <= 0; _a += _by1)
{
if(!(_a == 7))
{
printf("%d", _a);
printf("\n");
}
}
_a = 0;
do
{
_a = (_a + 1);
if(!(_a == 6) && _a != 8)
{
printf("%d", _a);
printf("\n");
}
} while(_a < 14);
_a = 0;
while(_a < 12)
{
printf("%d", _a);
_a = (_a + 1);
}
printf("\n");
return 0;
}
