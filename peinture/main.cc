#include <iostream>

#include "entry.hh"
#include "commands.hh"

long countDiff(int l, int h)
{
  long res = 0;
  for (int i = 0; i < l; i++)
	for (int j = 0; j < h; j++)
		if (current[i][j] != entry[i][j])
			res++;
  return res;
}

int main (int argc, char **argv)
{
std::cout << countDiff(716, 1523) << std::endl;
	for (int i = 0; i < 716 ; i++)
	{
		for (int j = 0; j < 1523 ; j++)
		{
			if (entry[i][j] == '#')
			{
				//paintsq(i, j, 0);
			}
		}
	}
	return 0;
}



