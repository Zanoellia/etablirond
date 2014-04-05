
def parse():
  intersections = []
  streets = []
  inputfile = open ('paris_54000.txt')

# first line
  line = inputfile.readline()
  data = line.split()
  nb_intersections = int(data[0])
  nb_streets = int(data[1])
  nb_secs = int(data[2])
  nb_cars = int(data[3])
  init_inter = int(data[4])

# data
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
  return [nb_intersections, nb_streets, nb_secs, nb_cars, init_inter,
          intersections, streets]

