#include <stdio.h>
void main(void)
{
register int _by;
float  _r3, _r2, _r1;
_r1 = -2.4;
_r2 = -34.989;
_r3 = (_r1 * (_r2 / 7.4));
printf("%f", _r3);
printf("\n");
scanf("%f", &_r1);
fseek(stdin, 0, SEEK_END);
_r3 = (_r1 + _r3);
printf("%f", _r3);
printf("\n");
}
