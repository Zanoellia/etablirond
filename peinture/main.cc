#include <iostream>

#include "entry.hh"
#include "commands.hh"

int main (int argc, char **argv)
{
	for (int i = 0; i < 716 ; i++)
	{
		for (int j = 0; j < 1523 ; j++)
		{
			if (entry[i][j] == '#')
			{
				paintsq(i, j, 1);
			}
		}
	}
	return 0;
}



