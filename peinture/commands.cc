#include "commands.hh"

extern const char entry[716][1523];
extern char current[716][1523];

/* Coordonées not checked */
void paintsq(int r, int c, int s)
{
  for (int y = r - s; y <= r + s; y++)
    for (int x = c - s; x <= c + s; x++)
      current[y][x] = '#';
}


void erasecell(int r, int c)
{
  current[r][c] = '.';
}

