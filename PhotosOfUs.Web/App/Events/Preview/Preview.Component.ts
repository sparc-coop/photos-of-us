namespace PhotosOfUs {
    angular.module('app')
        .component('preview', {
            templateUrl: '/Events/Preview',
            bindings: {
                event: '<'
            }
        });
}
