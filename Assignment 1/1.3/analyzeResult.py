import numpy as np

class Tokendata:
    def __init__(self, word, count, class0, class1):
        self.word = word
        self.count = int(count)
        self.class0 = int(class0)
        self.class1 = int(class1)
    
    def __str__(self):
        return self.word
    
    def Probability(self, class0Count, class1Count, vocabularySize):
        """
        Returns the probability of the word being in class 0 and class 1 with laplace smoothing, without using logaritms.
        """
        prob0 = (self.class0 + 1) / (class0Count + vocabularySize)
        prob1 = (self.class1 + 1) / (class1Count + vocabularySize)

        return prob0*10**4, prob1*10**4
    
class Review:
    def __init__(self, review, label):
        self.review = review
        self.label = label

    def __str__(self):
        output = " ".join(word for word in self.review)
        return str(self.label) + " | " + output
    
def ReadVocabulary(filename):
    vocabulary = dict()
    class0Count = 0
    class1Count = 0

    with open(filename, 'r') as file:
        for line in file:
            content = line.split()
            if len(content) == 4:
                vocabulary[content[0]] = Tokendata(content[0], content[1], content[2], content[3])
                class0Count += int(content[2])
                class1Count += int(content[3])
            else:
                class0Count += int(content[2-1])
                class1Count += int(content[3-1])

    return vocabulary, class0Count, class1Count

def ReadTrainingSet(filename):
    dataSet = []
    with open(filename, 'r') as file:
        for line in file:
            line = line.split()
            sentence = Review(line[:-1], int(line[-1]))
            dataSet.append(sentence)
    return dataSet

fileVocabulary = "trainedVocabulary.txt"
fileTrainingSet = "RestaurantReviewsTrainingSet.txt"
investigatedWords = ['friendly', 'perfectly', 'horrible', 'poor']

vocabulary, class0Count, class1Count = ReadVocabulary(fileVocabulary)
dataSet = ReadTrainingSet(fileTrainingSet)

# Find posterior probabilities
class0 = 0
class1 = 0

for review in dataSet:
    if review.label == 0:
        class0 += 1
    else:
        class1 += 1

numberOfWords = 0
for word in vocabulary:
    numberOfWords += vocabulary[word].count


print("Class 0: ", class0/len(dataSet), class0Count/numberOfWords)
print("Class 1: ", class1/len(dataSet), class1Count/numberOfWords)

for word in investigatedWords:
    if word in vocabulary:
        print("Word: ", word)
        print(" > Count: ", vocabulary[word].count)
        print(" > Class 0: ", vocabulary[word].class0)
        print(" > Class 1: ", vocabulary[word].class1)
        print(" > Probability: ", vocabulary[word].Probability(class0Count, class1Count, len(vocabulary)))
        print()
    else:
        print("Word not found in vocabulary")


