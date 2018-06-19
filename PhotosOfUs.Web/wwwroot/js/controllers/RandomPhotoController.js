app.controller('RandomPhotoCtrl', ['$scope', '$window', '$location', '$http', ($scope, $window, $location, $http) => {

$scope.msg = 'testing';
$scope.testMsg = () => {
    console.log("hiii");
    $scope.msg = 'hi';
};

    //$scope.getRandomPhoto = () => {

    //    let id = 21;
    //$http.get('/api/Photo/' + id).then(x => {
    //    $scope.photo = x.data;
    //    console.log("hello??", photo);


    //    function getRandomInt(min, max) {
    //        return Math.floor(Math.random() * (max - min + 1) + min);
    //    }

    //    //let area = document.getElementsByClassName("main-landing")[0];
    //    let getRandomPosition = (element) => {
    //        var x = document.body.offsetHeight - element.clientHeight;
    //        var y = document.body.offsetWidth - element.clientWidth;
    //        //var randomX = Math.floor(Math.random() * x);
    //        //var randomY = Math.floor(Math.random() * y);
    //        let coords = [getRandomInt(0, 900), getRandomInt(400, y)];
    //        console.log(coords);
    //        return coords;
    //    }
    //    let loadImages = () => {
    //        var img = document.createElement('img');
    //        img.setAttribute("style", "position:absolute; border-radius: 20px;");
    //        img.setAttribute("src", "~/images/photo/background.jpeg");
    //        document.body.appendChild(img);
    //        var xy = getRandomPosition(img);
    //        img.style.top = xy[0] + 'px';
    //        img.style.left = xy[1] + 'px';
    //    }

    //    var counter = 0;
    //    var interval = setInterval(function () {
    //        counter += 1;
    //        if (counter === 10) {
    //            clearInterval(interval);
    //        }
    //        loadImages();
    //    }, 2000);
    //})
    //};

}]);