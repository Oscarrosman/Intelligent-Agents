import numpy as np
import matplotlib.pyplot as plt

class Token():
    def __init__(self, word, index, weight, class0, class1) -> None:
        self.word = word
        self.index = index
        self.count = class0 + class1
        self.weight = weight
        self.class0 = class0
        self.class1 = class1

    def __lt__(self, other):
        return self.weight < other.weight
    
    def __str__(self) -> str:
        return f'{self.word} {self.index} {self.weight} {self.class0} {self.class1}'

    

def ReadAccuracies(filename):
    accuracies = []
    with open(filename, 'r') as file:
        for line in file:
            line = line.replace(',', '.')
            line = line.split(" ")
            accuracy = [float(x) for x in line]
            accuracy[0] = int(accuracy[0])
            accuracies.append(accuracy)
    return accuracies

def ReadVocabulary(filename):
    vocabulary = []
    with open(filename, 'r') as file:
        for line in file:
            line = line.replace(',', '.')
            word, index, weight, class0, class1 = line.split(" ")
            vocabulary.append(Token(word, int(index), float(weight), int(class0), int(class1)))
    return vocabulary

def PlotAccuracies(accuracies):
    trainingAccuracy = [100*x[1] for x in accuracies]
    validationAccuracy = [100*x[2] for x in accuracies]
    epochs = [x[0] for x in accuracies]

    plt.plot(epochs, trainingAccuracy, color='green', label='Training Accuracy')
    plt.plot(epochs, validationAccuracy, color='red', label='Validation Accuracy')
    plt.xlabel('Epochs')
    plt.ylabel('Accuracy [%]')
    plt.title('Training and Validation Accuracy')
    plt.xlim(0, 1000)
    #plt.ylim(50, 120)
    plt.legend(labels=[f'Training Accuracy, highest: {max(trainingAccuracy):3.0f}%', f'Highest validation Accuracy: {max(validationAccuracy):.2f}%'])
    plt.show()

def PresentVocabulary(vocabulary):
    vocabulary.sort(reverse=True)

    # 10 most positive words:
    print("\n10 most positive words:")
    print("Word".ljust(15) + "    | Index| Weight   | Class 0 | Class 1")
    print("-" * 50)
    for i, word in enumerate(vocabulary[:10]):
        print(f"{i+1}. {word.word.ljust(15)} | {word.index:4d} | {word.weight:.3f}  | {word.class0:4d} | {word.class1:4d}")

    vocabulary.sort()
    # 10 most negative words:
    print("\n10 most negative words:")
    print("Word".ljust(15) + "    | Index| Weight   | Class 0 | Class 1")
    print("-" * 50)
    for i, word in enumerate(vocabulary[:10]):
        print(f"{i+1}. {word.word.ljust(15)} | {word.index:4d} | - {abs(word.weight):.3f}  | {word.class0:4d} | {word.class1:4d}")


def PrintShortWords(vocabulary):
    for word in vocabulary:
        if len(word.word) < 5:
            print(word.word)

accuracy = ReadAccuracies("exportedResults.txt")
vocabulary = ReadVocabulary("exportedVocabulary.txt")


PresentVocabulary(vocabulary)
#PlotAccuracies(accuracy)
#PrintShortWords(vocabulary)
