#include <stdio.h>
int main(void)
{
int  _a;
_a = 1;
do
{
printf("%d", _a);
_a = (_a + 1);
} while(_a <= 0);
printf("\n");
return 0;
}
