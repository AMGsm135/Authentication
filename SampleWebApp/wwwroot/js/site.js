const authApiUrl = "http://api.gimex.com/api/v1";
const selfApiUrl = "http://app.gimex.com/api";

document.addEventListener("DOMContentLoaded",
    function() {
        document.querySelector("#reload-captcha").addEventListener("click", reloadCaptcha);
        document.querySelector("#signin").addEventListener("click", signin);
        document.querySelector("#signout").addEventListener("click", signout);
        document.querySelector("#refresh-token").addEventListener("click", tokenRefresh);

        document.querySelector("#user-info").addEventListener("click", userInfo);
        document.querySelector("#api-call").addEventListener("click", apiCall);

        reloadCaptcha();
    });


async function reloadCaptcha() {
    const captchaResult = (await get(authApiUrl + "/captcha")).content;
    document.querySelector("#captcha-image").src = "data:image/png;base64," + captchaResult.captchaImage;
    document.querySelector("#captcha-id").value = captchaResult.captchaId;
}

async function signin() {
    const username = document.querySelector("#username").value;
    const password = document.querySelector("#password").value;
    let captchaId = document.querySelector("#captcha-id").value;
    let captchaCode = document.querySelector("#captcha-code").value;

    const result = await post(authApiUrl + "/signin/password",
        {
            username: username,
            password: password,
            captchaId: captchaId,
            captchaCode: captchaCode
        });
    document.querySelector("#login-result").innerText = JSON.stringify(result);
    await reloadCaptcha();
}

async function signout() {

    const result = await post(authApiUrl + "/signout", null);
    document.querySelector("#login-result").innerText = JSON.stringify(result);
}

async function tokenRefresh() {

    const result = await post(authApiUrl + "/signin/refresh", null);
    document.querySelector("#login-result").innerText = JSON.stringify(result);
}

async function userInfo() {

    const result = await get(authApiUrl + "/check/user");
    document.querySelector("#api-result").innerText = JSON.stringify(result);
}

async function apiCall() {

    const result = await get(selfApiUrl + "/request/info");
    document.querySelector("#api-result").innerText = JSON.stringify(result);
}





async function get(address, query) {
    const response = await fetch(address,
        {
            method: 'GET',
            credentials: 'include',
            crossDomain: true
        });
    return response.json();
}

async function post(address, data) {
    const response = await fetch(address,
        {
            method: 'POST',
            credentials: 'include',
            crossDomain: true,
            body: JSON.stringify(data),
            headers: {
                'Content-Type': 'application/json'
            }
        });
    return response.json();
}

