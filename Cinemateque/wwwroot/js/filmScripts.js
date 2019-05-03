function getActors() {
    var uri = "actors"
    $.ajax({
        type: "GET",
        url: uri,
        beforeSend: function (xhr) {
            var token = getCookie("Token");
            xhr.setRequestHeader("Authorization", "Bearer " + token);
        },
        success: function (data) {
            getTable("actors", data)
        }
    });      
}

function getDirectors() {
    var uri = "directors"
    $.ajax({
        type: "GET",
        url: uri,
        beforeSend: function (xhr) {
            var token = getCookie("Token");
            xhr.setRequestHeader("Authorization", "Bearer " + token);
        },
        success: function (data) {
            getTable("directors", data)
        }
    });
}

function getFilms() {
    var uri = "films"
    $.ajax({
        type: "GET",
        url: uri,
        beforeSend: function (xhr) {
            var token = getCookie("Token");
            xhr.setRequestHeader("Authorization", "Bearer " + token);
        },
        success: function (data) {
            getTable("films", data)
        }
    });
}

function searchFilm() {
    var name = document.getElementById("nameSearch").value;
    var genre = document.getElementById("genreSearch").value;
    var actors = document.getElementById("actorSearch").value;
    var director = document.getElementById("directorSearch").value;
    var dat = JSON.stringify({
        "Name": name,
        "Genre": genre,
        "Director": director,
        "Actor" : actors
    });

    $.ajax({
        type: 'POST',
        url: "search",
        data: dat,
        contentType: "application/json",
        beforeSend: function (xhr) {
            var token = getCookie("Token");
            xhr.setRequestHeader("Authorization", "Bearer " + token);
        },
        success: function (data) {
            getTable("films", data)
        },
        error: function (data) {
            console.log(data)
        },

    });
}

function getTable(selector, data) {
    var table = document.getElementById(selector)
    var tr = table.insertRow(-1);    

    var col = [];
    for (var i = 0; i < data.length; i++) {
        for (var key in data[i]) {
            if (col.indexOf(key) === -1) {
                col.push(key);
            }
        }
    }

    for (var i = 0; i < col.length; i++) {
        var th = document.createElement("th");    
        th.innerHTML = col[i];
        tr.appendChild(th);
    }

    for (var i = 0; i < data.length; i++) {

        tr = table.insertRow(-1);

        for (var j = 0; j < col.length; j++) {
            var tabCell = tr.insertCell(-1);
            tabCell.innerHTML = data[i][col[j]];
        }
    }
}

function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}
