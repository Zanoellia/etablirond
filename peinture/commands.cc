#include "commands.hh"

extern char entry[716][1522];

/* Coordonées not checked */
void paintsq(int r, int c, int s)
{
  for (int y = r - s; y <= r + s; y++)
    for (int x = c - s; x <= c + s; x++)
      entry[y][x] = '#';
}

void erasecell(int r, int c)
{
  entry[r][c] = '.';
}
