import numpy as np
import random
from sympy import primefactors

def binary_to_decimal(binary):
    decimal = 0
    power = 0
    while binary > 0:
        decimal += (binary % 10) * (2 ** power)
        binary //= 10
        power += 1
    return decimal

def day_of_week(years, days, currentDay):
    weekdays = ['Friday', 'Saturday', 'Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday']
    days = years*365 # Days from years
    days += years//4 # Leap years
    currentYear = 2024
    while currentYear < currentYear + years:
        if currentYear % 100 == 0:
            days -= 1
        if currentYear % 400 == 0:
            days += 1

        currentYear += 1
    days += days

    index = weekdays.index(currentDay)
    day = weekdays[index + days % 7]
    return day

def b2d(input):
    input = input.split(' ')
    input = reversed(input)
    decimal = 0
    for power, b in enumerate(input):
        decimal += int(b) * (2 ** (power))
    return decimal

binary = '0 0 1 1 0 1 0 1 1 1 0 1'

print(b2d(binary)/3)
print(7*3*41)

alphabet = 'abcdefghijklmnopqrstuvwxyz'

def GenerateRandom(voc):
    copy1 = [x for x in voc]
    crypto1 = [[x, y] for x, y in zip(voc, np.random.permutation(copy1))]
    crypto = dict(crypto1)
    return crypto

def Encrypt(text, crypto):
    print('Original: ', text)
    encrypted = ''
    text = text.lower()
    for x in text:
        if x in crypto:
            encrypted += crypto[x]
        else:
            encrypted += x
    print('Encrypted: ', encrypted)
    return encrypted

def Decrypt(text, crypto):
    decrypted = ''
    for x in text:
        if x in crypto.values():
            for key, value in crypto.items():
                if value == x:
                    decrypted += key
        else:
            decrypted += x
    print('Decrypted: ', decrypted)
    return decrypted

#crypto = GenerateRandom(alphabet)
#text = 'Product'
#text1 = 'court'
#encrypted = Encrypt(text, crypto)
#encrypted1 = Encrypt(text1, crypto)
#decrypted = Decrypt(encrypted, crypto)

def day_of_week(years, days, currentDay):
    weekdays = ['Friday', 'Saturday', 'Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday']
    days = years*365 # Days from years
    days += years//4 # Leap years
    originalYear = 2024
    currentYear = originalYear
    while currentYear < originalYear + years:
        if currentYear % 100 == 0:
            days -= 1
        if currentYear % 400 == 0:
            days += 1

        currentYear += 1
    days += days

    index = weekdays.index(currentDay)
    day = weekdays[(index + days % 7)%len(weekdays)]
    return day

day = day_of_week(89, 100, 'Thursday')
print('Hello world')
print('Day: ', day)
print((365*43 + 43 // 4 + 43)%7)
print((365*89 + 89//4 -1  + 100)%7 )

def decimal_to_binary(decimal):
    binary = bin(decimal)[2:]
    print(f'Decimal ({decimal}) converted to binary: {binary}')
    return binary

randomNumber = random.randint(0, 1000)



decimal_to_binary(randomNumber)
factors = primefactors(randomNumber)
print(f"The prime factors of {randomNumber} are: {factors}")

    
crypto = GenerateRandom(alphabet)
text = 'Complicated'
text1 = 'Adept'
encrypted = Encrypt(text, crypto)
encrypted1 = Encrypt(text1, crypto)
decrypted = Decrypt(encrypted, crypto)

x = 'count the  letters including whitespace in this sentence'
print(f'Sentence length: {len(x)}')

print((27)*365 + 6)




