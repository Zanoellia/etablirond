
def parse():
#nb_intersection = 11348
#nb_street = 17958
#nb_sec = 54000
#nb_car = 8
#init_inter = 4516

  intersections = []
  streets = []
  inputfile = open ('paris_54000.txt')

  my_text = inputfile.readlines()[1:] # pass first line
  line_nb = 0

# we suppressed the first line
  for line in my_text:
    if line.find('.') != -1:
      data = line.split()
      intersections.append([float(data[0]), float(data[1])])
    else:
      data = line.split()
      streets.append([int(data[0]), int(data[1]), int(data[2]),
                      int(data[3]), int(data[4])])
      line_nb = line_nb + 1

      inputfile.close()
      intersections.reverse()
      streets.reverse()
      return [intersections, streets]

