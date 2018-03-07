$(function () {

    $("#HabiticaLogin").validate({

        rules: {
            Email: {

                required: true,
                Email: true
            },
            Password: {

                required: true,
                Password: true
            },

            messages: {
                Email: {

                    required: 'Please enter an email address blah blah blah.',
                    Email: 'Please enter a <em>Valid</em> email address.'
                }

            }

        }
    });

});