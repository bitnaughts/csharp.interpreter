using System;

class HashManager {
    //The purpose of this class is to assign unique hashes to objects to correlate them between seperate scripts
    static int hash_counter = 0;

    public static int getNewHash() {
        return hash_counter++;
    }
    public static void retireHash() {

    }
}