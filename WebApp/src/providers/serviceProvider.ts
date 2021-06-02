import axios from 'axios';
import { IAuthUser } from "../features/authentication/redux/interfaces";

//axios.defaults.withCredentials = true; //include all cookies

const LOCAL_ENDPOINT = 'https://localhost:5001/';
const PRODUCTION_ENDPOINT = 'http://cosc2649a3test.ap-southeast-2.elasticbeanstalk.com/';

export const sendRequestForResult = (
    action: string,
    auth: IAuthUser | null,
    jsonData: any,
    formData: any = null,
    method: string = 'POST'
) => {
    const authHeaders = auth
        ? {
            'AccountId': auth.accountId,
            'Authorization': `Bearer ${ auth.authToken }`
        }
        : {};

    const acceptHeaders = { 'Accept': 'application/json'};
    const contentHeaders = formData
        ? { 'Content-Type': 'multipart/form-data' }
        : { 'Content-Type': 'application/json' };

    const headers = {
        ...authHeaders,
        ...contentHeaders,
        ...acceptHeaders,
    };

    const body = method === 'POST' || method === 'PUT'
        ? (
            jsonData ? JSON.stringify(jsonData) : formData
        )
        : null;

    const requestOptions : any = {
        timeout: 180000, // 3 minutes
        method: method,
        url: PRODUCTION_ENDPOINT + action,
        headers: headers,
        data: body
    };

    return axios(requestOptions).then((result: any) => {
        if (result.status !== 200)
            return result.json().then((error: any) => {
                throw error;
            })
        else
            return result.data;
    });
};