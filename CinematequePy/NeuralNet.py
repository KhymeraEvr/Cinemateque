from os import listdir
from os.path import isfile, join
import numpy as np
import pandas as pd
import tensorflow as tf
import Regression
import datetime
import csv
import re
import matplotlib.pyplot as plt

from tensorflow import feature_column
from tensorflow.keras import layers
from sklearn.model_selection import train_test_split

actorsLen = 10
crewLen = 7
genresLen = 19
companiesLen = 48

# Training parameters.
learning_rate = 0.001
training_steps = 2
batch_size = 256
display_step = 100

# Network parameters.
n_hidden_1 = 128 # 1st layer number of neurons.
n_hidden_2 = 256 # 2nd layer number of neurons.

class MovieData:
    pass

class ActorData:
    pass

def average(listt):
    rats = list(map(lambda x: x.Rating, listt))
    sumr = sum(rats)
    avr = sumr/len(rats)
    return avr

def sumUpCrewData(inPath, files, releaseDate):
    results = []
    for file in files:
        path = join(inPath, file + '.csv')
        try:
            data = pd.read_csv(path)
        except FileNotFoundError:
            file = re.sub(r'[^A-Za-z0-9_. ]+', '', file)
            path = join(inPath, file.replace(" ", "") + '.csv')
            data = pd.read_csv(path)
        dataM = []
        for index, row in data.iterrows():
            ob = ActorData()
            ob.Rating = row.Rating
            ob.Date = datetime.datetime.strptime(row.Date, '%d/%m/%Y')
            dataM.append(ob)
        dataM = sorted(dataM, key=lambda x: x.Date)
        recentRanks = list(filter(lambda x: x.Date <= releaseDate, dataM))
#        for i in range (0, 8):
#            recentRanks.insert(0, recentRanks[recentRanks.__len__() - 1])
 #       for i in range(0, 3):
  #          recentRanks.insert(0, recentRanks[recentRanks.__len__() - 2])
   #     for i in range(0, 2):
    #        recentRanks.insert(0, recentRanks[recentRanks.__len__() - 2])

        avr = average(recentRanks)
        results.append(avr)
    return  results


def processMovieCsv(names):
    movies = []
    for name in names:
        try:
            movData = pd.read_csv(name, header=None)
            actors = movData[3][0].split(';')
            actorsPop = movData[4][0].split(';')
            crew = movData[5][0].split(';')
            crewPop = movData[6][0].split(';')
            budget = movData[8][0]
            genreFlags = movData[10][0].split(';')
            companiesFlags = movData[11][0].split(';')
            target = movData[13][0]
            if(target == 0): continue;
            releaseDate = datetime.datetime.strptime(movData[12][0], '%Y-%m-%d')
            actorsAv = sumUpCrewData('../ActorsCsvs',actors, releaseDate)

            movieData = MovieData();
            setattr(movieData, 'target', int(target * 10) )
            setattr(movieData, 'budget', budget)
            for i in range (0, actorsLen):
                setattr(movieData, 'actorAv'+ str(i), actorsAv[i])
            for i in range(0, actorsLen):
                setattr(movieData, 'actorPo'+ str(i), float(actorsPop[i]))
            crewAv = sumUpCrewData('../CrewCsvs', crew, releaseDate)
            for i in range (0, crewLen):
                setattr(movieData, 'crewAv'+ str(i), crewAv[i])
            for i in range(0, crewLen):
                setattr(movieData, 'crewPo' + str(i), float(crewPop[i]))
           # for i in range(0, genresLen):
            #    setattr(movieData, 'genre' + str(i), float(genreFlags[i]))
            #for i in range(0, companiesLen):
             #   setattr(movieData, 'comp' + str(i), float(companiesFlags[i]))
            movies.append(movieData)
        except: continue
    return movies

def createTrainData():
    files = [f for f in listdir('../MoviesCsvs') if isfile(join('../MoviesCsvs', f))]
    files = list(map(lambda x: '../MoviesCsvs/' + x, files))
    moviesModels = processMovieCsv(files)

    with open("combined_csv.csv", 'w', newline='', encoding='utf8') as f:
        writer = csv.writer(f)
        writer.writerow(moviesModels[0].__dict__.keys())
        for mv in moviesModels:
            writer.writerow(mv.__dict__.values())

def createPredictData(file):
    files = ['../MoviesCsvs/' + file]
    moviesModels = processMovieCsv(files)

    with open(file, 'w', newline='', encoding='utf8') as f:
        writer = csv.writer(f)
        writer.writerow(moviesModels[0].__dict__.keys())
        for mv in moviesModels:
            writer.writerow(mv.__dict__.values())

def df_to_dataset(dataframe, shuffle=True, batch_size=32):
  dataframe = dataframe.copy()
  labels = dataframe.pop('target')
  ds = tf.data.Dataset.from_tensor_slices((dict(dataframe), labels))
  ds = ds.repeat().shuffle(5000).batch(batch_size).prefetch(1)
  return ds

def input_fn(features, labels, training=True, batch_size=100):
    # Convert the inputs to a Dataset.
    dataset = tf.data.Dataset.from_tensor_slices((dict(features), labels))

    # Shuffle and repeat if you are in training mode.
    if training:
        dataset = dataset.shuffle(1000).repeat()

    return dataset.batch(batch_size)



def train():
    #createTrainData();
    dataframe = pd.read_csv("combined_csv.csv")
    dataframe.head()
    train, test = train_test_split(dataframe)
    y_train = train.pop('target')
    y_eval = test.pop('target')

    # train.actorAv0.hist(bins=20)
    # train.budget.hist(bins=20)
    # y_train.hist(bins=20)
    # fig, ax = plt.subplots()
    # y_train.hist( ax=ax)
    # fig.savefig('example.png')
    NUMERIC_COLUMNS = [ 'actorAv0', 'actorAv1', 'actorAv2', 'actorAv3', 'actorAv4', 'actorAv5', 'actorAv6', 'actorAv7',
                       'actorAv8', 'actorAv9', 'actorPo0', 'actorPo1', 'actorPo2', 'actorPo3', 'actorPo4', 'actorPo5',
                       'actorPo6', 'actorPo7', 'actorPo8', 'actorPo9', 'crewAv0', 'crewAv1', 'crewAv2', 'crewAv3',
                       'crewAv4', 'crewAv5', 'crewAv6', 'crewPo0', 'crewPo1', 'crewPo2', 'crewPo3', 'crewPo4',
                       'crewPo5', 'crewPo6']

    feature_columns = []

    for feature_name in NUMERIC_COLUMNS:
        feature_columns.append(tf.feature_column.numeric_column(feature_name, dtype=tf.float32))

    print(feature_columns)

    classifier = tf.estimator.DNNClassifier(
        feature_columns=feature_columns,
        hidden_units=[150, 300],
        n_classes=101)

    classifier.train(
        input_fn=lambda: input_fn(train, y_train, training=True),
        steps=5000)

    eval_result = classifier.evaluate(
        input_fn=lambda: input_fn(test, y_eval, training=False))

    print('\nTest set accuracy: {accuracy:0.8f}\n'.format(**eval_result))

    return classifier


#def predict(fileName):
createPredictData("12YearsaSlave.csv")
dataframe = pd.read_csv("12YearsaSlave.csv")
dataframe.head()
y_train = dataframe.pop('target')
classifier = train();
ress = {}
prerRes = classifier.predict(input_fn=lambda: input_fn(dataframe, y_train, training=True))
for single_prediction in prerRes:
    numb = 0
    for i in single_prediction['probabilities']:
        print(str(numb) + '         ' +str(i))
        ress[str(numb)] = str(i)
        numb = numb + 1
    break;
print(ress)
    #return ress

   # return prerRes;