function auth() {
    var name = document.getElementById("emailLogin").value;
    var password = document.getElementById("passwordLogin").value;
    var loginData = {
        Username: name,
        Password: password,
    };


    $.ajax({
        type: "POST",
        url: 'authenticate',
        contentType: "application/json",
        data: JSON.stringify(loginData),
        success: function (inputData) {
            $('.userName').text(inputData.username);
            $('#userRole').text(inputData.role);
            $('.userInfo').css('display', 'block');
            $('.loginForm').css('display', 'none');
            setCookie("Token", inputData.token);
            console.log(dats);
        },
        fail: function (data) {
            console.log(data);
        }
    })
}

function setCookie(cname, cvalue) {
    var d = new Date();
    d.setTime(d.getTime() + (24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
}