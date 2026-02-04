<template>
    <div class="modal fade" id="staticRegister" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" aria-labelledby="staticRegisterLabel" aria-hidden="true">
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
          <div class="modal-header bg-violet">
            <h1 class="modal-title fs-5 text-white" id="staticRegisterLabel">Account Registration</h1>
            <button @click="closeRegistration" type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
          </div>
          <div class="modal-body">
            <form id="registrationForm" @submit.prevent="submitRegistration" autocomplete="off">
              <div class="row">
                <div class="col">
                  <div class="mb-3">
                    <input type="text" required v-model="firstname" class="form-control border-violet" placeholder="enter First Name" autocomplete="off" />
                  </div>
                </div>
                <div class="col">
                  <div class="mb-3">
                    <input type="text" required v-model="lastname" class="form-control border-violet" placeholder="enter Last Name" autocomplete="off" />
                  </div>
                </div>
              </div>
              <div class="row">
                <div class="col">
                  <div class="mb-3">
                    <input type="email" required class="form-control border-violet" v-model="email" placeholder="enter Email Address" autocomplete="off">
                  </div>
                </div>
                <div class="col">
                  <div class="mb-3">
                    <input type="text" required class="form-control border-violet" v-model="mobile" placeholder="enter Mobile No." autocomplete="off">
                  </div>
                </div>
              </div>
              <div class="row">
                <div class="col">
                  <div class="mb-3">
                    <input type="text" required class="form-control border-violet" v-model="username" placeholder="enter User Name" autocomplete="off">
                  </div>
                </div>
                <div class="col">
                  <div class="mb-3">
                    <input type="password" required class="form-control border-violet" v-model="password" placeholder="enter Password" autocomplete="off">
                  </div>
                </div>
              </div>
    
              <button type="submit" class="btn btn-violet text-white mx-1">register</button>
              <button type="reset" class="btn btn-violet text-white">reset</button>
    
            </form>
    
          </div>
          <div class="modal-footer">
              <div class="w-100 text-left text-danger">{{ registerMsg }}</div>
          </div>
        </div>
      </div>
    </div>
    </template>
    
<script lang="ts">
import { defineComponent } from 'vue';
import jQuery from 'jquery';
import axios from 'axios';

// Hot Chocolate typically serves GraphQL at the /graphql endpoint
const api = axios.create({
  baseURL: "http://localhost:5094/graphql",
  headers: {
    'Accept': 'application/json',
    'Content-Type': 'application/json'
  }
});

export default defineComponent({
  name: 'Register-Page',
  data() {
    return {
      firstname: '',
      lastname: '',
      email: '',
      mobile: '',
      username: '',
      password: '',
      registerMsg: ''
    };
  },
  methods: {
    closeRegistration() {
      this.firstname = '';
      this.lastname = '';
      this.email = '';
      this.mobile = '';
      this.username = '';
      this.password = '';
      this.registerMsg = '';
      jQuery("#registerReset").trigger('click');
    },
    async submitRegistration() {
      this.registerMsg = "Please wait...";
      const signupPayload = {
        query: `
          mutation Signup($input: SignupInput!) {
            signup(input: $input) {
              message
            }
          }
        `,
        variables: {
          input: { // Must match the $input variable name in your mutation
            firstname: this.firstname,
            lastname: this.lastname,
            email: this.email,
            mobile: this.mobile,
            username: this.username,
            password: this.password
          }
        }
      };

      try {
        const res = await api.post('/graphql', signupPayload); 
        
        if (res.data.errors) {
          this.registerMsg = res.data.errors[0].message;
        } else {
          const result = res.data.data?.signup; 
          if (result?.message) {
            this.registerMsg = result.message;
          }
        }
      } catch (error: any) {
        this.registerMsg = error.response?.data?.errors?.[0]?.message || error.message || "An error occurred";
      } finally {
        setTimeout(() => { this.registerMsg = ''; }, 3000);
      }
    }
  }
});
</script>
    
