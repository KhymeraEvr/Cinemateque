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

function getUsers() {
    var uri = "user"
    $.ajax({
        type: "GET",
        url: uri,
        beforeSend: function (xhr) {
            var token = getCookie("Token");
            xhr.setRequestHeader("Authorization", "Bearer " + token);
        },
        success: function (data) {
            getTable("Users", data)
        }
    });
}

function getUserFilms() {
    var uri = "UserFilms"
    $.ajax({
        type: "GET",
        url: uri,
        beforeSend: function (xhr) {
            var token = getCookie("Token");
            xhr.setRequestHeader("Authorization", "Bearer " + token);
        },
        success: function (data) {
            getTable("UserFilms", data)
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

function getBestDirector() {
    var date = document.getElementById("startDate").value
    var uri = "bestDirector/" + date

    $.ajax({
        type: "GET",
        url: uri,
        beforeSend: function (xhr) {
            var token = getCookie("Token");
            xhr.setRequestHeader("Authorization", "Bearer " + token);
        },
        success: function (data) {
            document.getElementById("bestDirectorName").innerHTML = data.name
            document.getElementById("bestDirectorRate").innerHTML = data.rate
            document.getElementById("bestDirector").style.display = "block"
        }
    });
}

function getTopDirector() {
    var date = document.getElementById("startDate").value
    var uri = "ratedDirector"

    $.ajax({
        type: "GET",
        url: uri,
        beforeSend: function (xhr) {
            var token = getCookie("Token");
            xhr.setRequestHeader("Authorization", "Bearer " + token);
        },
        success: function (data) {
            document.getElementById("topDirectorName").innerHTML = data.name
            document.getElementById("topDirectorRate").innerHTML = data.rate
            document.getElementById("topDirector").style.display = "block"
        }
    });
}

function getPopGenre() {
    var date = document.getElementById("startDate").value
    var uri = "popGenre/" + date

    $.ajax({
        type: "GET",
        url: uri,
        beforeSend: function (xhr) {
            var token = getCookie("Token");
            xhr.setRequestHeader("Authorization", "Bearer " + token);
        },
        success: function (data) {
            document.getElementById("popularGenreName").innerHTML = data.name            
            document.getElementById("popularGenre").style.display = "block"
        }
    });
}

function getActiveUser() {
    var date = document.getElementById("startDate").value
    var uri = "activeUser/" + date

    $.ajax({
        type: "GET",
        url: uri,
        beforeSend: function (xhr) {
            var token = getCookie("Token");
            xhr.setRequestHeader("Authorization", "Bearer " + token);
        },
        success: function (data) {
            document.getElementById("activeUserName").innerHTML = data.name
            document.getElementById("activeUser").style.display = "block"
        }
    });
}

function getFavorites() {
    var uri = "favorites";

    $.ajax({
        type: "GET",
        url: uri,
        beforeSend: function (xhr) {
            var token = getCookie("Token");
            xhr.setRequestHeader("Authorization", "Bearer " + token);
        },
        success: function (data) {
            document.getElementById("favorites").innerHTML = "Your favorite actor -  " + data.actor + " director -   " + data.director + "  genre -   " + data.genre;
        }
    });
}

function getDate() {
   var date = document.getElementById("startDate").value;
   return date;
}


function watchLater(data) {
    var filmId = $(data).data('assigned-id');
    var uri = "later/" + filmId
    $.ajax({
        type: "GET",
        url: uri,
        beforeSend: function (xhr) {
            var token = getCookie("Token");
            xhr.setRequestHeader("Authorization", "Bearer " + token);
        },
        success: function (data) {
            alert('Added to watch later')
        }
    });
}

function rate(data) {
    var filmId = $(data).data('assigned-id');
    var uri = "/rate/" + filmId + "/" + data.value
    $.ajax({
        type: "GET",
        url: uri,
        beforeSend: function (xhr) {
            var token = getCookie("Token");
            xhr.setRequestHeader("Authorization", "Bearer " + token);
        },
        success: function (data) {
            alert('Rated')
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
        "Actor": actors
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
            document.getElementById('maybe').innerHTML = data;
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

function deleteFilm(data) {
    var id = $(data).data('assigned-id');
    return fetch('delete/' + id, {
        method: 'delete'
    }).then(response =>
        alert("Successfuly deleted")
    );
}

function addToCart(data) {
    var prodName = $(data).data('assigned-id');
    $.ajax({
        type: "GET",
        url: "Home/AddToCart?prodId=" + prodName,
        success: function (data) {
            alert("Successfuly added to cart")
        }
    });
}
