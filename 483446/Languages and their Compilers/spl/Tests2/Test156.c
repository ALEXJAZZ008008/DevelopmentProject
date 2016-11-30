#include <stdio.h>
int main(void)
{
float  _a;
register float _by1;
for(_a = 0.1; _by1 = 0.1, (_a - 0.5) * ((_by1 > 0) - (_by1 < 0)) <= 0; _a += _by1)
{
printf("%f", _a);
printf("\n");
}
printf("\n");
return 0;
}
