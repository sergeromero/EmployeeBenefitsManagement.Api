import { FormBuilder, Validators } from '@angular/forms'

export function createLoginForm(formBuilder: FormBuilder) {
    return formBuilder.nonNullable.group({
        email: [
            '',
            [
                Validators.required,
                Validators.email,
                Validators.maxLength(255)
            ]
        ],
        password: [
            '',
            [
                Validators.required,
                Validators.minLength(5),
                Validators.maxLength(100)
            ]
        ]
    });
}