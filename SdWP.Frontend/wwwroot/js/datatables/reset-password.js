window.resetPassword = {
    register: function () {
        $('#resetPasswordBtn').on('click', function (e) {
            e.preventDefault();
            const PasswordModal = new bootstrap.Modal(document.getElementById('ChangePasswordModal')).show();
        });
    }
};