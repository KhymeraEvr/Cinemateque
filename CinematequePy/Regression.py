import numpy as np
import matplotlib.pyplot as plt
import pandas as pd
from sklearn.model_selection import train_test_split
from sklearn.linear_model import LinearRegression
from sklearn.preprocessing import PolynomialFeatures
from datetime import datetime

class ActorData:
    pass

class Regres:
    def __init__(self, fileName):
        self.fileName = fileName
        self.subjectName = fileName.format(self.fileName.split('.csv')[0])

    def readFile(self):
        self.dataset = pd.read_csv("../{}".format(self.fileName))
        self.X = []
        self.y = []
        dataM = []

        for index, row in self.dataset.iterrows():
            ob = ActorData()
            ob.Rating = row.Rating
            ob.Date = datetime.strptime(row.Date, '%d/%m/%Y')
            dataM.append(ob)
        dataM = sorted(dataM, key=lambda x: x.Date)
        for i in range(0, len(dataM)):
            try:
                if(dataM[i].Rating == 0): continue;
                total_seconds = dataM[i].Date.timestamp()
                self.X.append([total_seconds])
                self.y.append( dataM[i].Rating)
            except: continue


    # Splitting the dataset into the Training set and Test set
    def splitSet(self):
        self.X_train, self.X_test, self.y_train, self.y_test = train_test_split(self.X, self.y, test_size=0.2, random_state=0)

    # Visualizing the Linear Regression results
    def viz_linear(self):
        lin_reg = LinearRegression()
        lin_reg.fit(self.X, self.y)
        plt.scatter(self.X, self.y, color='red')
        plt.plot(self.X, lin_reg.predict(self.X), color='blue')
        plt.title('Truth or Bluff (Linear Regression)')
        plt.xlabel('Position level')
        plt.ylabel('Salary')
        plt.show()
        return;

    # Visualizing the Polymonial Regression results
    def polymonial(self, value):
        poly_reg = PolynomialFeatures(degree=4)
        X_poly = poly_reg.fit_transform(self.X)
        pol_reg = LinearRegression()
        pol_reg.fit(X_poly, self.y)
        plt.scatter(self.X, self.y, color='red')
        plt.plot(self.X, pol_reg.predict(poly_reg.fit_transform(self.X)), color='blue')
        plt.title("Rating progression for {}".format(self.subjectName))
        plt.xlabel('Date')
        plt.ylabel('Rating')
        plt.savefig("DataGraphs\{}.png".format(self.subjectName))

        # Predicting a new result with Polymonial Regression
        predicted = pol_reg.predict(poly_reg.fit_transform([[value]]))
        print(predicted)
        return predicted
        # output should be 52588245.92078003

    def predictValue( self ):
        self.readFile()
        self.splitSet()
        result = self.polymonial( len(self.X) )

        return result