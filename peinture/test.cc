#include <iostream>
#include "commands.hh"

char correct[5][8] = {"....#..",
		      "..###..",
		      "..#.#..",
		      "..###..",
		      "..#...."};

char entry[5][8] = {".......",
	       ".......",
	       ".......",
	       ".......",
	       "......."};


int main()
{
  paintsq(2, 3, 1);
  paintsq(0, 4, 0);
  paintsq(4, 2, 0);
  erasecell(2, 3);


  for(int i = 0; i < 5; i++)
    std::cout << entry[i] << std::endl;
}
