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
			  int score = 0;

			  for (int counti = 0; counti < 3; counti++)
			    for (int countj = 0; countj < 3; countj++)
			      if (entry[i][j] == '#' && current[i][j] == '.')
				score++;
			  if (score > 5)
			  {
			    paintsq(i + 1, j + 1, 1);
			    std::cout << "PAINTSQ " << i + 1 << " " << j + 1 << " " <<  1 << std::endl;
			  }
			  std::cout << "PAINTSQ " << i << " " << j  << " " << 0 << std::endl;
			  count++;
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



