#include <stdio.h>
int main(void)
{
float  _r3, _r2, _r1;
_r1 = -2.4;
_r2 = -34.989;
_r3 = (_r1 * (_r2 / 7.4));
printf("%f", _r3);
printf("\n");
scanf(" %f", &_r1);
_r3 = (_r1 + _r3);
printf("%f", _r3);
printf("\n");
return 0;
}
