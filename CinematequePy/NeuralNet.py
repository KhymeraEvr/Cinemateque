from os import listdir
from os.path import isfile, join
import numpy as np
import pandas as pd

import tensorflow as tf

from tensorflow import feature_column
from tensorflow.keras import layers
from sklearn.model_selection import train_test_split

files = [f for f in listdir('../MoviesCsvs') if isfile(join('../MoviesCsvs', f))]
files = list(map(lambda x: '../MoviesCsvs/' + x, files))
print(files)
combined_csv = pd.concat([pd.read_csv(f) for f in files], sort=False)
combined_csv.to_csv( "combined_csv.csv", index=False, encoding='utf-8-sig')
print(combined_csv)