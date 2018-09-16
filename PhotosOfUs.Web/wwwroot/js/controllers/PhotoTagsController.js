app.controller('PhotoTagCtrl', TagCtrl);

    function TagCtrl($timeout, $q) {
        var self = this;

        self.tags = [
            {
                'tag_title': 'Travel'
            }
        ];

        self.newTag = function (chip) {
            return {
                tag_title: chip
            };
        };
    }
//})();
