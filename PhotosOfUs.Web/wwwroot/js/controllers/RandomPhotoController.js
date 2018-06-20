app.controller('RandomPhotoCtrl', ['$scope', '$window', '$location', '$http', ($scope, $window, $location, $http) => {

    function getRandomInt(min, max) {
        return Math.floor(Math.random() * (max - min + 1) + min);
    }

    //let area = document.getElementsByClassName("main-landing")[0];
    $scope.getRandomPosition = (element) => {
        var x = 900 - element.clientHeight;
        var y = document.body.offsetWidth - element.clientWidth;
        //var randomX = Math.floor(Math.random() * x);
        //var randomY = Math.floor(Math.random() * y);
        let coords = [getRandomInt(0, x), getRandomInt(600, 1000)];
        console.log(coords);
        return coords;
    }

    $scope.loadImages = (ph) => {
        var img = document.createElement('img');
        img.setAttribute("src", ph.url || "https://photosofus.blob.core.windows.net/photos/4/20170721-062830-IMG_000820180505111110.JPG");
        img.setAttribute("style", "position:absolute; border-radius: 20px; max-width: 400px; max-height: 400px;");
        document.body.appendChild(img);
        var xy = $scope.getRandomPosition(img);
        img.style.top = xy[0] + 'px';
        img.style.left = xy[1] + 'px';
    }

    $scope.getRandomPhoto = () => {
        
            let counter = 0;
            let interval = setInterval(function () {
                counter += 1;
                if (counter === 10) {
                    clearInterval(interval);
                }
                let idList = [1, 2, 3, 28, 5, 6, 7, 29, 9, 30, 31, 19, 57, 21, 22, 23, 24, 58, 59, 60];
                $scope.id = idList[getRandomInt(0, idList.length-1)];
                console.log($scope.id)
                $http.get('/api/Photo/' + $scope.id).then(x => {
                    $scope.photo = x.data;
                    console.log("hello??", $scope.photo);
                    $scope.loadImages($scope.photo);
                })
                }, 2000);
        
    }; 

}]);