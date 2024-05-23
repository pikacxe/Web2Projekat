import { useState } from "react";

export const useLocalStorage = (keyName:string,defaultValue:any) => {
    const [storedValue, setStoredValue] = useState(() => {
        try{
            const value = window.localStorage.getItem(keyName);
            if(value){
                return JSON.parse(value);
            }
            else{
                window.localStorage.setItem(keyName,JSON.stringify(defaultValue));
                return defaultValue;
            }
        }
        catch(err){
            console.error(`Error while getting ${keyName} from localStorage.`);
            return defaultValue;
        }
    })
    const setValue = (newValue:any) => {
        try{
            window.localStorage.setItem(keyName,JSON.stringify(newValue));
        }
        catch(err){
            console.error(`Error while setting value to ${keyName} in localStorage`);
        }
        setStoredValue(newValue);
    };
    return [storedValue, setValue];
}