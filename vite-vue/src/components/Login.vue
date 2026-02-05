<template>
    <div>
    <div className="modal fade" id="staticLogin" data-bs-backdrop="static" data-bs-keyboard="false" tabIndex={-1} aria-labelledby="staticLoginLabel" aria-hidden="true">
        <div className="modal-dialog modal-sm modal-dialog-centered">
          <div className="modal-content">
            <div className="modal-header bg-primary">
              <h1 className="modal-title fs-5 text-white" id="staticLoginLabel">User's SignIn</h1>
              <button @click="closeLogin" type="button" className="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
              <button id="closeLogin"  type="button" className="btn-close d-none" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div className="modal-body">
              <form @submit.prevent="submitLogindata" autoComplete='off'>
                  <div className="mb-3">
                      <input type="text" required v-model="username" :disabled="isDisabled" className="form-control border-primary" id="uname" placeholder="enter Username"/>
                  </div>            
                  <div className="mb-3">
                      <input type="password" required v-model="password" :disabled="isDisabled" className="form-control border-primary" id="pword" placeholder="enter Password"/>
                  </div>            
                  <button type="submit" className="btn btn-primary mx-1" :disabled="isDisabled">signin</button>
                  <button id="loginReset" type="reset" @click="resetLogin"  className="btn btn-primary">reset</button>
                  <button id="mfa" type="button" className="btn btn-primary mx-1 d-none" data-bs-toggle="modal" data-bs-target="#staticMfa">mfa</button>            
              </form>
            </div>
            <div className="modal-footer">
              <div className='w-100 text-center text-danger'>{{message}}</div>
            </div>
          </div>
        </div>
      </div>        
      <Mfa/>
    </div>
</template>

<script lang="ts">
import axios from 'axios';
import jQuery from 'jquery';
import Mfa from './Mfa.vue';
import { defineComponent } from 'vue';

const api = axios.create({
  baseURL: "http://localhost:5094",
  headers: {'Accept': 'application/json',
            'Content-Type': 'application/json'}
});

export default defineComponent({
    name: 'Login-Page',
    components: {
        Mfa
    },
    data() {
        return {
            username: '',
            password: '',
            message: '',
            isDisabled: false
        };
    },
    methods: {
       submitLogindata: async function() {
            this.isDisabled = true;
            this.message = "please wait...";

            const signinPayload = {
                query: `
                mutation Signin($input: SigninInput!) {
                    signin(input: $input) {
                        id
                        firstname
                        lastname
                        email
                        mobile
                        username      
                        isactivated
                        isblocked 
                        profilepic
                        rolename
                        qrcodeurl
                        token
                        message
                    }
                }
                `,
                variables: {
                    input: { 
                        username: this.username,
                        password: this.password
                    }
                }
            };

            try {
                const res = await api.post('/graphql', signinPayload); 
                
                if (res.data.errors) {
                    this.message = res.data.errors[0].message;
                    return;
                } else {
                    const result = res.data.data?.signin; 
                    this.message = result.message;
                    if (result.qrcodeurl === null) {
                        window.sessionStorage.setItem('USERID', result.id);
                        window.sessionStorage.setItem("USERNAME", result.username)
                        let userpic: string = `http://localhost:5094/users/${result.profilepic}`;
                        window.sessionStorage.setItem("USERPIC", userpic);
                        window.sessionStorage.setItem('TOKEN', result.token);
                        setTimeout(() => {
                            this.resetLogin();
                            jQuery("#closeLogin").trigger('click'); 
                            location.reload();                           
                        }, 3000);
                    } else {
                        window.sessionStorage.setItem('USERID', result.id);
                        let userpic: string = `http://localhost:5094/users/${result.profilepic}`;
                        window.sessionStorage.setItem("USERPIC", userpic);
                        window.sessionStorage.setItem('TOKEN', result.token);
                        setTimeout(() => {
                            this.resetLogin();
                            jQuery("#closeLogin").trigger('click');                            
                        }, 3000);
                    }
                    
                }
            } catch (error: any) {
                this.message = error.response?.data?.errors?.[0]?.message || error.message || "An error occurred";
            } finally {
                setTimeout(() => { this.message = ''; }, 3000);
                this.isDisabled = false;
            }
        },  
        closeLogin: function() {
            this.message = '';
            this.isDisabled = false;
            sessionStorage.removeItem('USERID');
            sessionStorage.removeItem('USERNAME');
            sessionStorage.removeItem('TOKEN');
            sessionStorage.removeItem('USERPIC');
            jQuery("#loginReset").trigger('click');
        },
        resetLogin: function() {
            this.message = '';
            this.username = '';
            this.password = '';
            this.isDisabled = false;
        }
    },



});
</script>