class HomeClass {

    constructor() {
        this.initialise();
    }

    public initialise() {

        toastr.options = { "positionClass": "toast-top-center", "newestOnTop": true };
        $("#fileUploadBtn").off("click").on("click", () => this.fileUpload());
        $("#fileUploader").off("change").on("change", (event) => this.fileUploadSelected(event));
    }

    private fileUpload() {
        $("#fileUploadNameTxt").val('');
        $("#fileUploader").click();
    }

    private fileUploadSelected(event: JQueryEventObject) {
        var input = <HTMLInputElement>event.target;
        var file = input.files[0];
        var fileNameLength = file.name.length;
        var fileNameExtension = file.name.substring((fileNameLength - 4), (fileNameLength));
        if (!(fileNameLength > 0 && fileNameExtension == ".pre")) {

            toastr["error"]("File type is not supported, please select a '.pre' file.");

        } else {

            $("#messagePanel").removeClass('hidden');

            var $form = $(event.currentTarget).parents('form');
            var model = new FormData();
            model.append("UploadedFile", file);

            $("#fileUploadNameTxt").val(file.name);

            $.ajax({
                dataType: 'json',
                url: $form.attr('action'),
                method: 'Post',
                data: model,
                cache: false,
                contentType: false,                
                processData: false,
                success: (result) => {
                    if (result.code == 409) {
                        toastr["error"]("The file format is incorrect.  The value for 'Years' is not set.", "Error");
                    } else {
                        toastr["success"]("The data is saved to the database", "Success");
                        $("#messagePanel").addClass('hidden');
                        $("#messageSuccessDatabaseName").html(result.value);
                        $("#messageSuccessPanel").removeClass('hidden');
                    }
                }
            });
        }

    }
  
}
var home = new HomeClass();