window.cryptojs = {
    async encryptGCM(plainBytes, passwordBytes, salt, iv, iterations) {
        const keyMaterial = await crypto.subtle.importKey(
            "raw",
            passwordBytes,
            { name: "PBKDF2" },
            false,
            ["deriveKey"]
        );

        const key = await crypto.subtle.deriveKey(
            {
                name: "PBKDF2",
                salt: new Uint8Array(salt),
                iterations: iterations,
                hash: "SHA-256"
            },
            keyMaterial,
            { name: "AES-GCM", length: 256 },
            false,
            ["encrypt"]
        );

        const encrypted = await crypto.subtle.encrypt(
            {
                name: "AES-GCM",
                iv: new Uint8Array(iv),
            },
            key,
            plainBytes
        );

        return new Uint8Array(encrypted);
    },

    async decryptGCM(encryptedBytes, passwordBytes, salt, iv, iterations) {
        const keyMaterial = await crypto.subtle.importKey(
            "raw",
            passwordBytes,
            { name: "PBKDF2" },
            false,
            ["deriveKey"]
        );

        const key = await crypto.subtle.deriveKey(
            {
                name: "PBKDF2",
                salt: new Uint8Array(salt),
                iterations: iterations,
                hash: "SHA-256"
            },
            keyMaterial,
            { name: "AES-GCM", length: 256 },
            false,
            ["decrypt"]
        );

        const decrypted = await crypto.subtle.decrypt(
            {
                name: "AES-GCM",
                iv: new Uint8Array(iv),
            },
            key,
            encryptedBytes
        );

        return new Uint8Array(decrypted);
    },

    async generateBytes(length) {
        const bytes = new Uint8Array(length);
        crypto.getRandomValues(bytes); 
        return bytes;
    }

};
