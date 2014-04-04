#include <iostream>
#include <string.h>

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
	int count = 0;
	memset(current, '.', 716*1523*sizeof(char));
	for (int i = 0; i < 716 ; i++)
	{
		for (int j = 0; j < 1523 ; j++)
		{
			if (entry[i][j] == '#' && current[i][j] == '.')
			{
				int counti = 1, countj = 1;
				while (entry[i + counti][j] == '#' && current[i + counti][j] != '#') counti++;
				while ((j + countj) < 716 && entry[i][j + countj] == '#'&& entry[i][j+ countj] != '#') countj++;
				int min = std::min(counti, countj)/2;
				paintsq(i + min, j + min, min);
				count++;
				std::cout << "PAINTSQ " << i + min << " " << j + min << " " <<  min << std::endl;

			}
			if (entry[i][j] == '.' && current[i][j] == '#')
			{
				erasecell(i, j);
				std::cout << "ERASECELL " << i << " " << j << std::endl;
				count++;
			}
		}
	}
	std::cout << count << std::endl;
	return 0;
}



