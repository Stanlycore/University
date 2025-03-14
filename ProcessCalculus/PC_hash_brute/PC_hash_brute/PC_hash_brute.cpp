#include <iostream>
#include <string>
#include <thread>
#include "SHA512.h"

SHA512 sha512;
sha512.hash("Example"); 

// Функция для вычисления хэша
std::string calculateHash(const std::string& password) {
    unsigned char hash[SHA512_DIGEST_LENGTH];
    SHA512_CTX sha512;
    SHA512_Init(&sha512);
    SHA512_Update(&sha512, password.c_str(), password.size());
    SHA512_Final(hash, &sha512);

    std::string hashString;
    for (int i = 0; i < SHA512_DIGEST_LENGTH; i++) {
        char hex[3];
        sprintf(hex, "%02x", hash[i]);
        hashString += hex;
    }

    return hashString;
}

// Функция для подбора хэша
void findPassword(const std::string& start, const std::string& end, const std::string& targetHash, const std::string& chars) {
    for (size_t i = 0; i < chars.size(); i++) {
        for (size_t j = 0; j < chars.size(); j++) {
            for (size_t k = 0; k < chars.size(); k++) {
                for (size_t l = 0; l < chars.size(); l++) {
                    for (size_t m = 0; m < chars.size(); m++) {
                        for (size_t n = 0; n < chars.size(); n++) {
                            for (size_t o = 0; o < chars.size(); o++) {
                                std::string password = std::string(1, chars[i]) + std::string(1, chars[j]) + std::string(1, chars[k]) + std::string(1, chars[l]) + std::string(1, chars[m]) + std::string(1, chars[n]) + std::string(1, chars[o]);
                                if (password >= start && password <= end) {
                                    if (calculateHash(password) == targetHash) {
                                        std::cout << "Пароль найден: " << password << std::endl;
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

int main() {
    std::string targetHash;
    std::cout << "Введите хэш sha512: ";
    std::cin >> targetHash;

    std::string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
    std::thread threads[16];

    for (int i = 0; i < 16; i++) {
        std::string start = std::string(7, chars[0]);
        std::string end = std::string(7, chars[chars.size() - 1]);
        if (i < 15) {
            end = std::string(i / 2, chars[0]) + std::string(1, chars[(i % 2) * (chars.size() - 1)]) + std::string(6 - i / 2, chars[chars.size() - 1]);
        }

        threads[i] = std::thread(findPassword, start, end, targetHash, chars);
    }

    for (int i = 0; i < 16; i++) {
        threads[i].join();
    }

    return 0;
}
