use std::ffi::{c_char, CStr, CString};
use std::ptr;

// traditional Rust function for greeting
fn greet_rust(name: &str) -> String {
    format!("Hello, {name}! Rust here.")
}

// C-compatible wrapper for the Rust greeting function
#[unsafe(no_mangle)]
pub unsafe extern "C" fn greet(name: *const c_char) -> *mut c_char {
    if name.is_null() {
        return ptr::null_mut();
    }

    let Ok(name) = (unsafe { CStr::from_ptr(name) }).to_str() else {
        return ptr::null_mut();
    };

    match CString::new(greet_rust(name)) {
        Ok(result) => result.into_raw(),
        Err(_) => ptr::null_mut(),
    }
}


// C-compatible wrapper for freeing the allocated greeting string
#[unsafe(no_mangle)]
pub unsafe extern "C" fn free_greeting(result: *mut c_char) {
    if !result.is_null() {
        drop(unsafe { CString::from_raw(result) });
    }
}
