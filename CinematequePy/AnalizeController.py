import flask
import Regression
import NeuralNet
import json

app = flask.Flask(__name__)
app.config["DEBUG"] = True


@app.route('/regres/<filename>', methods=['GET'])
def regres(filename):
    serv = Regression.Regres(filename)
    result = serv.predictValue(0);
    resultString = str(result).strip('[]')

    return resultString

@app.route('/NN/<filename>', methods=['GET'])
def predictMovie(filename):
    result = NeuralNet.predict(filename)
    print(result)
    return result

app.run()