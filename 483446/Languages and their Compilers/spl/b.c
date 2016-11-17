#include <stdio.h>
void main(void)
{
register int _by;
int  _b, _a;
char  _c;
float  _e, _d;
scanf(" %d", &_a);
scanf(" %d", &_b);
if(_a > _b)
{
printf("A");
}
else
{
printf("B");
}
printf("\n");
scanf(" %f", &_d);
_e = (_d * 2.3);
printf("%f", _e);
printf("\n");
scanf(" %c", &_c);
printf("%c", _c);
printf("\n");
}
